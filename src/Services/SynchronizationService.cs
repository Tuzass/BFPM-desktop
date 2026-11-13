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

    public static async Task PullChanges(AppDbContext context, ApiClient apiClient, int userID, DomainEntityType entityType, int entityID)
    {
        switch (entityType)
        {
            case DomainEntityType.Project:
                var project = await DomainService.GetProjectByID(context, entityID);
                var projectResponse = await apiClient.GetProject(project.RemoteID!.Value);

                Change change = new(userID, DomainEntityType.Project, entityID, null, null, null, null, null, false);
                bool anyChanged = false;

                if (project.Name != projectResponse.Name)
                {
                    change.Name = projectResponse.Name;
                    anyChanged = true;
                }

                if (project.Description != projectResponse.Description)
                {
                    change.Description = projectResponse.Description;
                    anyChanged = true;
                }

                if (project.Status != projectResponse.Status)
                {
                    change.Status = projectResponse.Status switch
                    {
                        ProjectStatus.Active => DomainEntityStatus.Active,
                        ProjectStatus.OnHold => DomainEntityStatus.OnHold,
                        ProjectStatus.Archived => DomainEntityStatus.Archived,
                        ProjectStatus.Completed => DomainEntityStatus.Completed,
                        _ => DomainEntityStatus.Active,
                    };

                    anyChanged = true;
                }

                if (anyChanged)
                {
                    await DomainService.ApplyChange(context, change);
                    
                    while (project.Version < projectResponse.Version)
                    {
                        project.IncreaseVersion();
                    }
                }

                break;

            default:
                throw new InvalidOperationException("Unexpected entity type.");
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
