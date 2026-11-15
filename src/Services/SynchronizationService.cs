using Domain.Entities;
using Domain.Enums;
using Infrastructure.Database;
using Infrastructure.Entities;
using Infrastructure.Enums;
using Workflow.Enums;
using API.Responses;
using Microsoft.EntityFrameworkCore;

namespace Services;

public static class SynchronizationService
{
    private static async Task<Dictionary<EntityIdPair, int>> GetEntityVersions(AppDbContext context, List<EntityIdPair> pairs)
    {
        Dictionary<EntityIdPair, int> entityVersions = [];

        foreach (var pair in pairs)
        {
            int version = await DomainService.GetEntityVersionByRemoteID(context, pair.EntityType, pair.ID);

            if (version == -1)
            {
                throw new InvalidOperationException($"Entity ({pair.EntityType}, {pair.ID}) not found in local DB");
            }

            entityVersions[pair] = version;
        }

        return entityVersions;
    }

    private static async Task<Change?> GetProjectChanges(int userID, Project localProject, FullProjectResponse remoteProject)
    {
        Change change = new(userID, DomainEntityType.Project, localProject.ID!.Value, null, null, null, null, null, false);
        bool anyChanged = false;

        if (localProject.Name != remoteProject.Name)
        {
            change.Name = remoteProject.Name;
            anyChanged = true;
        }

        if (localProject.Description != remoteProject.Description)
        {
            change.Description = remoteProject.Description;
            anyChanged = true;
        }

        if (localProject.Status != remoteProject.Status)
        {
            change.Status = remoteProject.Status switch
            {
                ProjectStatus.Active => DomainEntityStatus.Active,
                ProjectStatus.OnHold => DomainEntityStatus.OnHold,
                ProjectStatus.Archived => DomainEntityStatus.Archived,
                ProjectStatus.Completed => DomainEntityStatus.Completed,
                _ => DomainEntityStatus.Active,
            };

            anyChanged = true;
        }

        if (!anyChanged)
        {
            return null;
        }

        return change;
    }

    private static async Task<Change?> GetTaskChanges(int userID, TaskItem localTask, TaskResponse remoteTask)
    {
        Change change = new(userID, DomainEntityType.Task, localTask.ID!.Value, null, null, null, null, null, false);
        bool anyChanged = false;

        if (localTask.Name != remoteTask.Name)
        {
            change.Name = remoteTask.Name;
            anyChanged = true;
        }

        if (localTask.Description != remoteTask.Description)
        {
            change.Description = remoteTask.Description;
            anyChanged = true;
        }

        if (localTask.Status != remoteTask.Status)
        {
            change.Status = remoteTask.Status switch
            {
                TaskItemStatus.Todo => DomainEntityStatus.Todo,
                TaskItemStatus.InProgress => DomainEntityStatus.InProgress,
                TaskItemStatus.OnHold => DomainEntityStatus.OnHold,
                TaskItemStatus.Blocked => DomainEntityStatus.Blocked,
                TaskItemStatus.Completed => DomainEntityStatus.Completed,
                _ => DomainEntityStatus.Todo,
            };

            anyChanged = true;
        }

        if (localTask.Priority != remoteTask.Priority)
        {
            change.Priority = remoteTask.Priority;
            anyChanged = true;
        }

        if (localTask.DueDate != remoteTask.DueDate)
        {
            change.DueDate = remoteTask.DueDate;
            change.DueDateChanged = true;
            anyChanged = true;
        }

        if (!anyChanged)
        {
            return null;
        }

        return change;
    }

    private static async Task<Change?> GetTagChanges(int userID, Tag localTag, TagResponse remoteTag)
    {
        Change change = new(userID, DomainEntityType.Tag, localTag.ID!.Value, null, null, null, null, null, false);
        bool anyChanged = false;

        if (localTag.Name != remoteTag.Name)
        {
            change.Name = remoteTag.Name;
            anyChanged = true;
        }

        if (!anyChanged)
        {
            return null;
        }

        return change;
    }

    public static async Task<bool> CommitChanges(AppDbContext context, ApiClient apiClient, int userID, int projectID, ActionType actionType, DomainEntityType targetType, int targetID, DomainEntityType? relatedEntityType, int? relatedEntityID, Change change)
    {
        var affectedEntitiesResponse = await apiClient.CreateAction(userID,
                                                                    projectID,
                                                                    actionType,
                                                                    targetType,
                                                                    targetID,
                                                                    relatedEntityType,
                                                                    relatedEntityID,
                                                                    change);

        Dictionary<EntityIdPair, int> entityVersions = await GetEntityVersions(context, affectedEntitiesResponse.AffectedEntities);
        var response = await apiClient.SendVersions(affectedEntitiesResponse.UserActionID, entityVersions);

        return response switch
        {
            "Approved" => true,
            "Rejected" => false,
            _ => throw new InvalidOperationException($"Unexpected user action status: {response}")
        };
    }

    public static async Task<bool> DeleteProject(AppDbContext context, ApiClient apiClient, int userID, int projectID)
    {
        var affectedEntitiesResponse = await apiClient.CreateAction(userID, projectID, ActionType.DeleteEntity, DomainEntityType.Project, projectID, null, null, null);

        Dictionary<EntityIdPair, int> entityVersions = await GetEntityVersions(context, affectedEntitiesResponse.AffectedEntities);
        var response = await apiClient.SendVersions(affectedEntitiesResponse.UserActionID, entityVersions);

        return response switch
        {
            "Approved" => true,
            "Rejected" => false,
            _ => throw new InvalidOperationException($"Unexpected user action status: {response}")
        };
    }

    public static async Task PullChanges(AppDbContext context, ApiClient apiClient, int userID, int projectID)
    {
        var project = await DomainService.GetProjectByID(context, projectID);
        var projectResponse = await apiClient.GetProject(project.RemoteID!.Value);

        using (var transaction = await context.Database.BeginTransactionAsync())
        {    
            try
            {
                var localTasks = await DomainService.GetTasksByProject(context, projectID);
                var localTags = await DomainService.GetTagsByProject(context, projectID);
                var localTaskTagLinks = await DomainService.GetTaskTagLinksByProject(context, projectID);

                var change = await GetProjectChanges(userID, project, projectResponse);

                if (change is not null)
                {
                    await DomainService.ApplyChange(context, change);
                }
                
                while (project.Version < projectResponse.Version)
                {
                    project.IncreaseVersion();
                }

                var tasksByRemoteID = localTasks
                    .Where(t => t.RemoteID.HasValue)
                    .ToDictionary(t => t.RemoteID!.Value);

                var tagsByRemoteID = localTags
                    .Where(t => t.RemoteID.HasValue)
                    .ToDictionary(t => t.RemoteID!.Value);

                foreach (var remoteTask in projectResponse.Tasks)
                {
                    if (!tasksByRemoteID.TryGetValue(remoteTask.ID, out var localTask))
                    {
                        localTask = await DomainService.CreateTaskFromRemote(context, projectID, remoteTask);
                        tasksByRemoteID.Add(remoteTask.ID, localTask);
                    }

                    else
                    {
                        change = await GetTaskChanges(userID, localTask, remoteTask);

                        if (change is not null)
                        {   
                            await DomainService.ApplyChange(context, change);
                        }

                        while (localTask.Version < remoteTask.Version)
                        {
                            localTask.IncreaseVersion();
                        }
                    }
                }

                foreach (var remoteTag in projectResponse.Tags)
                {
                    if (!tagsByRemoteID.TryGetValue(remoteTag.ID, out var localTag))
                    {
                        localTag = await DomainService.CreateTagFromRemote(context, projectID, remoteTag);
                        tagsByRemoteID.Add(remoteTag.ID, localTag);
                    }

                    else
                    {
                        change = await GetTagChanges(userID, localTag, remoteTag);

                        if (change is not null)
                        {   
                            await DomainService.ApplyChange(context, change);
                        }

                        while (localTag.Version < remoteTag.Version)
                        {
                            localTag.IncreaseVersion();
                        }
                    }
                }

                var remoteTaskIDs = projectResponse.Tasks
                    .Select(t => t.ID)
                    .ToHashSet();

                var remoteTagIDs = projectResponse.Tags
                    .Select(t => t.ID)
                    .ToHashSet();

                var orphanTasks = localTasks
                    .Where(t => t.RemoteID.HasValue && !remoteTaskIDs.Contains(t.RemoteID.Value))
                    .ToList();

                var orphanTags = localTags
                    .Where(t => t.RemoteID.HasValue && !remoteTagIDs.Contains(t.RemoteID.Value))
                    .ToList();

                foreach (var task in orphanTasks)
                {   
                    await DomainService.DeleteTask(context, task.ID!.Value);
                }

                foreach (var tag in orphanTags)
                {   
                    await DomainService.DeleteTag(context, tag.ID!.Value);
                }
                
                foreach (var link in localTaskTagLinks)
                {
                    context.TaskTagLinks.Remove(link);
                }

                await context.SaveChangesAsync();

                foreach (var link in projectResponse.TaskTagLinks)
                {
                    var task = tasksByRemoteID[link.TaskID];
                    var tag = tagsByRemoteID[link.TagID];
                    context.TaskTagLinks.Add(new(task.ID!.Value, tag.ID!.Value));
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }

            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    public static async Task<int> StoreRemoteProject(AppDbContext context, int assignedUserID, FullProjectResponse remoteProject)
    {
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                Project project = new(remoteProject.ID, remoteProject.OwnerID, assignedUserID, remoteProject.Name, remoteProject.Description, remoteProject.Status, remoteProject.CreatedAt, remoteProject.CompletedAt, remoteProject.Version);

                if (project.Status == ProjectStatus.Completed)
                {
                    project.Complete();
                }

                context.Projects.Add(project);
                await context.SaveChangesAsync();

                List<TaskItem> tasks = [];
                foreach (var task in remoteProject.Tasks)
                {
                    tasks.Add(new(task.ID, task.ProjectID, task.Name, task.Description, task.Status, task.Priority, task.DueDate, task.CreatedAt, task.CompletedAt, task.Version));
                }

                List<Tag> tags = [];
                foreach (var tag in remoteProject.Tags)
                {
                    tags.Add(new(tag.ID, tag.ProjectID, tag.Name, tag.Version));
                }

                context.Tasks.AddRange(tasks);
                context.Tags.AddRange(tags);
                await context.SaveChangesAsync();
                
                foreach (var link in remoteProject.TaskTagLinks)
                {
                    int taskID = tasks.Single(t => t.RemoteID == link.TaskID).ID!.Value;
                    int tagID = tags.Single(t => t.RemoteID == link.TagID).ID!.Value;
                    
                    context.TaskTagLinks.Add(new(taskID, tagID));
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return project.ID!.Value;
            }

            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    public static async Task<List<int>> StartupSynchronization(AppDbContext context, ApiClient apiClient, int userID)
    {
        var remoteProjectIDs = await apiClient.GetUserRemoteProjectIDs(userID);
        List<int> missingProjectIDs = [];

        foreach (var remoteProjectID in remoteProjectIDs)
        {
            if (!await DomainService.DoesUserHaveAssociatedProject(context, userID, remoteProjectID))
            {
                missingProjectIDs.Add(remoteProjectID);
                continue;
            }
        }

        var associatedProjects = await DomainService.GetUserAssociatedProjects(context, userID);
        List<int> orphanProjectIDs = [];

        foreach (var associatedProject in associatedProjects)
        {
            if (!remoteProjectIDs.Contains(associatedProject.RemoteID!.Value))
            {
                orphanProjectIDs.Add(associatedProject.ID!.Value);
            }
        }

        var missingProjects = await apiClient.GetProjectBatch(missingProjectIDs);
        foreach (var missingProject in missingProjects)
        {
            await StoreRemoteProject(context, userID, missingProject);
        }

        return orphanProjectIDs;
    }
}
