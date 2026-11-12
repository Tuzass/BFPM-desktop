using Domain.Entities;
using Domain.Enums;
using Infrastructure.Database;
using Infrastructure.Entities;
using Infrastructure.Enums;
using Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Services;

public static class DomainService
{
    public static async Task<bool> IsFolderOwnedByUser(AppDbContext context, int userID, int folderID)
    {
        return await context.Folders
            .AnyAsync(f => f.ID == folderID && f.OwnerID == userID);
    }

    public static async Task<bool> IsProjectOwnedByUser(AppDbContext context, int userID, int projectID)
    {
        return await context.Projects
            .AnyAsync(p => p.ID == projectID && p.OwnerID == userID);
    }

    public static async Task<bool> IsProjectLocal(AppDbContext context, int projectID)
    {
        var project = await GetProjectByID(context, projectID);
        return project.RemoteID is null;
    }

    public static async Task<bool> IsProjectAssignedToUser(AppDbContext context, int userID, int projectID)
    {
        return await context.Projects
            .AnyAsync(p => p.ID == projectID && p.AssignedUserID == userID);
    }

    public static async Task<bool> IsFolderDuplicate(AppDbContext context, int ownerID, string folderName, int? folderID = null)
    {
        return await context.Folders
            .AnyAsync(f => f.ID != folderID && f.OwnerID == ownerID && f.Name == folderName);
    }

    public static async Task<bool> IsProjectNameDuplicate(AppDbContext context, int ownerID, string projectName, int? projectID = null)
    {
        if (projectID is null)
        {
            return await context.Projects
                .AnyAsync(p => p.OwnerID == ownerID && p.Name == projectName);
        }

        var project = await GetProjectByID(context, projectID.Value);

        if (project.RemoteID is null)
        {
            return await context.Projects
                .AnyAsync(p => p.ID != projectID && p.RemoteID == null && p.OwnerID == ownerID && p.Name == projectName);
        }

        return await context.Projects
            .AnyAsync(p => p.ID != projectID && p.RemoteID != null && p.OwnerID == ownerID && p.Name == projectName);
    }

    public static async Task<bool> IsTaskDuplicate(AppDbContext context, int projectID, string taskName, int? taskID = null)
    {
        return await context.Tasks
            .AnyAsync(t => t.ID != taskID && t.ProjectID == projectID && t.Name == taskName);
    }

    public static async Task<bool> IsTagDuplicate(AppDbContext context, int projectID, string tagName, int? tagID = null)
    {
        return await context.Tags
            .AnyAsync(t => t.ID != tagID && t.ProjectID == projectID && t.Name == tagName);
    }

    public static async Task<int> GetEntityVersion(AppDbContext context, DomainEntityType entityType, int entityID)
    {
        switch (entityType)
        {
            case DomainEntityType.Project:
                var project = await GetProjectByID(context, entityID);
                return project.Version;
            
            case DomainEntityType.Task:
                var task = await GetTaskByID(context, entityID);
                return task.Version;
            
            case DomainEntityType.Tag:
                var tag = await GetTagByID(context, entityID);
                return tag.Version;
            
            default:
                return -1;
        }
    }

    public static async Task<Folder> GetFolderByID(AppDbContext context, int folderID)
    {
        var folder = await context.Folders
            .Where(f => f.ID == folderID)
            .SingleOrDefaultAsync();
        
        if (folder is null)
        {
            throw new InexistentFolderException();
        }

        return folder;
    }

    public static async Task<Project> GetProjectByID(AppDbContext context, int projectID)
    {
        var project = await context.Projects
            .Where(p => p.ID == projectID)
            .SingleOrDefaultAsync();
        
        if (project is null)
        {
            throw new InexistentProjectException();
        }

        return project;
    }

    public static async Task<TaskItem> GetTaskByID(AppDbContext context, int taskID)
    {  
        var task = await context.Tasks
            .Where(t => t.ID == taskID)
            .SingleOrDefaultAsync();
        
        if (task is null)
        {
            throw new InexistentTaskException();
        }

        return task;
    }

    public static async Task<Tag> GetTagByID(AppDbContext context, int tagID)
    {  
        var tag = await context.Tags
            .Where(t => t.ID == tagID)
            .SingleOrDefaultAsync();
        
        if (tag is null)
        {
            throw new InexistentTagException();
        }

        return tag;
    }

    public static async Task<Dictionary<string, ProjectRole>> GetProjectMembers(ApiClient apiClient, int projectID)
    {
        var projectResponse = await apiClient.GetProject(projectID);
        Dictionary<string, ProjectRole> assignedUsers = [];

        foreach (var user in projectResponse.AssignedUsers)
        {
            assignedUsers[user.UserName] = user.Role;
        }

        return assignedUsers;
    }

    public static async Task<List<string>> GetUsersAssignedToTask(ApiClient apiClient, int taskID)
    {
        var taskResponse = await apiClient.GetTask(taskID);
        return taskResponse.AssignedUsers.Values.ToList();
    }

    public static async Task<List<Folder>> GetFoldersContainingProject(AppDbContext context, int projectID)
    {
        return await context.FolderProjectLinks
            .Where(l => l.ProjectID == projectID)
            .Join(
                context.Folders,
                l => l.FolderID,
                f => f.ID,
                (l, f) => f)
            .ToListAsync();
    }

    public static async Task<List<Folder>> GetFoldersByName(AppDbContext context, int ownerID, string folderName)
    {
        return await context.Folders
            .Where(f => f.OwnerID == ownerID && f.Name.Contains(folderName))
            .ToListAsync();
    }

    public static async Task<List<Folder>> GetFoldersOwnedByUser(AppDbContext context, int userID)
    {
        return await context.Folders
            .Where(f => f.OwnerID == userID)
            .ToListAsync();
    }

    public static async Task<List<Project>> GetProjectsAssignedToUser(AppDbContext context, int userID)
    {
        return await context.Projects
            .Where(p => p.AssignedUserID == userID)
            .ToListAsync();
    }

    public static async Task<List<Project>> GetProjectsAssignedToFolder(AppDbContext context, int folderID)
    {
        return await context.FolderProjectLinks
            .Where(l => l.FolderID == folderID)
            .Join(
                context.Projects,
                l => l.ProjectID,
                p => p.ID,
                (l, p) => p)
            .ToListAsync();
    }

    public static async Task<List<Project>> GetProjectsNotAssignedToFolder(AppDbContext context, int userID, int folderID)
    {
        return await context.Projects
            .Where(p => p.AssignedUserID == userID)
            .Where(p => !context.FolderProjectLinks
                .Any(l => l.FolderID == folderID && l.ProjectID == p.ID))
            .ToListAsync();
    }

    public static async Task<List<TaskItem>> GetTasksByProject(AppDbContext context, int projectID)
    {
        return await context.Tasks
            .Where(t => t.ProjectID == projectID)
            .ToListAsync();
    }

    public static async Task<List<TaskItem>> GetTasksByTag(AppDbContext context, int tagID)
    {
        return await context.TaskTagLinks
            .Where(l => l.TagID == tagID)
            .Join(
                context.Tasks,
                l => l.TaskID,
                t => t.ID,
                (l, t) => t)
            .ToListAsync();
    }

    public static async Task<List<TaskItem>> GetTasksByStatus(AppDbContext context, int projectID, TaskItemStatus taskStatus)
    {
        return await context.Tasks
            .Where(t => t.ProjectID == projectID && t.Status == taskStatus)
            .ToListAsync();
    }

    public static async Task<List<TaskItem>> GetTasksByPriority(AppDbContext context, int projectID, TaskItemPriority taskPriority)
    {
        return await context.Tasks
            .Where(t => t.ProjectID == projectID && t.Priority == taskPriority)
            .ToListAsync();
    }

    public static async Task<List<TaskItem>> GetTasksByName(AppDbContext context, int projectID, string taskName)
    {
        return await context.Tasks
            .Where(t => t.ProjectID == projectID && t.Name.Contains(taskName))
            .ToListAsync();
    }

    public static async Task<List<TaskItem>> GetTasksAssignedToUser(ApiClient apiClient, int projectID, int userID)
    {
        var projectResponse = await apiClient.GetProject(projectID);
        List<TaskItem> assignedTasks = [];

        foreach (var task in projectResponse.Tasks)
        {
            if (task.AssignedUsers.ContainsKey(userID))
            {
                assignedTasks.Add(new(
                    task.ID,
                    null,
                    task.ProjectID,
                    task.Name,
                    task.Description,
                    task.Status,
                    task.Priority,
                    task.DueDate,
                    task.CreatedAt,
                    task.CompletedAt,
                    task.Version
                ));
            }
        }
        
        return assignedTasks;
    }

    public static async Task<List<Tag>> GetTagsByProject(AppDbContext context, int projectID)
    {
        return await context.Tags
            .Where(t => t.ProjectID == projectID)
            .ToListAsync();
    }

    public static async Task<List<Tag>> GetTagsByTask(AppDbContext context, int taskID)
    {
        return await context.TaskTagLinks
            .Where(l => l.TaskID == taskID)
            .Join(
                context.Tags,
                l => l.TagID,
                t => t.ID,
                (l, t) => t)
            .ToListAsync();
    }

    public static async Task<List<Tag>> GetTagsByName(AppDbContext context, int projectID, string tagName)
    {
        return await context.Tags
            .Where(t => t.ProjectID == projectID && t.Name.Contains(tagName))
            .ToListAsync();
    }

    public static async Task<List<TaskTagLink>> GetTaskTagLinksByProject(AppDbContext context, int projectID)
    {
        return await context.Tasks
            .Where(t => t.ProjectID == projectID)
            .Join(
                context.TaskTagLinks,
                t => t.ID,
                ttl => ttl.TaskID,
                (t, ttl) => ttl
            ).ToListAsync();
    }

    public static async Task CreateFolder(AppDbContext context, int ownerID, string folderName, string folderDescription)
    {
        bool isDuplicate = await context.Folders
            .AnyAsync(f => f.OwnerID == ownerID && f.Name == folderName);
        
        if (isDuplicate)
        {
            throw new DuplicateFolderException();
        }

        Folder newFolder = new(null, ownerID, folderName, folderDescription);
        context.Folders.Add(newFolder);
    }

    public static async Task CreateProject(AppDbContext context, int ownerID, string projectName, string projectDescription, ProjectStatus projectStatus)
    {
        bool isDuplicate = await context.Projects
            .AnyAsync(p => p.OwnerID == ownerID && p.Name == projectName);
        
        if (isDuplicate)
        {
            throw new DuplicateProjectException();
        }

        Project newProject = new(null, null, ownerID, ownerID, projectName, projectDescription, projectStatus, null, null);
        context.Projects.Add(newProject);
    }

    public static async Task CreateTask(AppDbContext context, int projectID, string taskName, string taskDescription, TaskItemStatus taskStatus, TaskItemPriority taskPriority, DateTime? taskDueDate)
    {
        bool isDuplicate = await context.Tasks
            .AnyAsync(t => t.ProjectID == projectID && t.Name == taskName);
        
        if (isDuplicate)
        {
            throw new DuplicateTaskException();
        }

        TaskItem newTask = new(null, null, projectID, taskName, taskDescription, taskStatus, taskPriority, taskDueDate, null, null);
        context.Tasks.Add(newTask);
    }

    public static async Task CreateTag(AppDbContext context, int projectID, string tagName)
    {
        bool isDuplicate = await context.Tags
            .AnyAsync(t => t.ProjectID == projectID && t.Name == tagName);
        
        if (isDuplicate)
        {
            throw new DuplicateTagException();
        }

        Tag newTag = new(null, null, projectID, tagName);
        context.Tags.Add(newTag);
    }

    public static async Task AssignProjectToFolder(AppDbContext context, int folderID, int projectID)
    {
        bool isDuplicate = await context.FolderProjectLinks
            .AnyAsync(l => l.FolderID == folderID && l.ProjectID == projectID);
        
        if (!isDuplicate)
        {
            FolderProjectLink newLink = new(folderID, projectID);
            context.FolderProjectLinks.Add(newLink);
        }
    }

    public static async Task RemoveProjectFromFolder(AppDbContext context, int folderID, int projectID)
    {
        bool linkExists = await context.FolderProjectLinks
            .AnyAsync(l => l.FolderID == folderID && l.ProjectID == projectID);
        
        if (linkExists)
        {
            FolderProjectLink link = new(folderID, projectID);
            context.FolderProjectLinks.Remove(link);
        }
    }

    public static async Task AssignTagToTask(AppDbContext context, int taskID, int tagID)
    {
        bool isDuplicate = await context.TaskTagLinks
            .AnyAsync(l => l.TaskID == taskID && l.TagID == tagID);

        if (isDuplicate)
        {
            return;
        }

        TaskTagLink newLink = new(taskID, tagID);
        context.TaskTagLinks.Add(newLink);
    }

    public static async Task RemoveTagFromTask(AppDbContext context, int taskID, int tagID)
    {
        bool linkExists = await context.TaskTagLinks
            .AnyAsync(l => l.TaskID == taskID && l.TagID == tagID);
        
        if (!linkExists)
        {
            return;
        }

        TaskTagLink link = new(taskID, tagID);
        context.TaskTagLinks.Remove(link);
    }

    public static async Task ApplyChange(AppDbContext context, Change change)
    {
        switch (change.TargetType)
        {
            case DomainEntityType.Folder:
                var folder = await context.Folders
                    .SingleOrDefaultAsync(f => f.ID == change.TargetID);
                
                if (folder is null)
                {
                    throw new InexistentFolderException();
                }

                if (change.Name is not null)
                {
                    folder.Name = change.Name;
                }

                if (change.Description is not null)
                {
                    folder.Description = change.Description;
                }

                if (change.Name is not null
                    || change.Description is not null)
                {
                    folder.IncreaseVersion();                        
                }

                break;
            
            case DomainEntityType.Project:
                var project = await context.Projects
                    .SingleOrDefaultAsync(f => f.ID == change.TargetID);
                
                if (project is null)
                {
                    throw new InexistentProjectException();
                }

                if (change.Name is not null)
                {
                    project.Name = change.Name;
                }

                if (change.Description is not null)
                {
                    project.Description = change.Description;
                }

                if (change.Status is not null)
                {
                    switch (change.Status)
                    {
                        case DomainEntityStatus.Active:
                            project.ChangeStatus(ProjectStatus.Active);
                            break;
                        
                        case DomainEntityStatus.Archived:
                            project.ChangeStatus(ProjectStatus.Archived);
                            break;
                        
                        case DomainEntityStatus.OnHold:
                            project.ChangeStatus(ProjectStatus.OnHold);
                            break;
                        
                        case DomainEntityStatus.Completed:
                            project.Complete();
                            break;
                        
                        default:
                            throw new UnexpectedProjectStatusException();
                    }
                }

                if (change.Name is not null
                    || change.Description is not null
                    || change.Status is not null)
                {
                    project.IncreaseVersion();                        
                }

                break;
            
            case DomainEntityType.Task:
                var task = await context.Tasks
                    .SingleOrDefaultAsync(f => f.ID == change.TargetID);
                
                if (task is null)
                {
                    throw new InexistentTaskException();
                }

                if (change.Name is not null)
                {
                    task.Name = change.Name;
                }

                if (change.Description is not null)
                {
                    task.Description = change.Description;
                }

                if (change.Status is not null)
                {
                    switch (change.Status)
                    {
                        case DomainEntityStatus.InProgress:
                            task.ChangeStatus(TaskItemStatus.InProgress);
                            break;
                        
                        case DomainEntityStatus.Todo:
                            task.ChangeStatus(TaskItemStatus.Todo);
                            break;
                        
                        case DomainEntityStatus.OnHold:
                            task.ChangeStatus(TaskItemStatus.OnHold);
                            break;
                        
                        case DomainEntityStatus.Blocked:
                            task.ChangeStatus(TaskItemStatus.Blocked);
                            break;
                        
                        case DomainEntityStatus.Completed:
                            task.Complete();
                            break;
                        
                        default:
                            throw new UnexpectedTaskStatusException();
                    }
                }

                if (change.Priority is not null)
                {
                    task.Priority = change.Priority switch
                    {
                        TaskItemPriority.Low => TaskItemPriority.Low,
                        TaskItemPriority.Medium => TaskItemPriority.Medium,
                        TaskItemPriority.High => TaskItemPriority.High,
                        TaskItemPriority.Urgent => TaskItemPriority.Urgent,
                        _ => throw new UnexpectedTaskPriorityException(),
                    };
                }

                if (change.DueDateChanged)
                {
                    task.ChangeDueDate(change.DueDate);
                }

                if (change.Name is not null
                    || change.Description is not null
                    || change.Status is not null
                    || change.Priority is not null
                    || change.DueDateChanged)
                {
                    task.IncreaseVersion();                        
                }

                break;
            
            case DomainEntityType.Tag:
                var tag = await context.Tags
                    .SingleOrDefaultAsync(f => f.ID == change.TargetID);
                
                if (tag is null)
                {
                    throw new InexistentTagException();
                }

                if (change.Name is not null)
                {
                    tag.Name = change.Name;
                    tag.IncreaseVersion();                        
                }

                break;
        }
    }

    public static async Task DeleteFolder(AppDbContext context, int folderID)
    {
        bool folderExists = await context.Folders
            .AnyAsync(f => f.ID == folderID);
        
        if (!folderExists)
        {
            return;
        }

        Folder? folder = await context.Folders
            .Where(f => f.ID == folderID)
            .SingleAsync();
        
        context.Folders.Remove(folder);
    }

    public static async Task DeleteProject(AppDbContext context, int projectID)
    {
        bool projectExists = await context.Projects
            .AnyAsync(f => f.ID == projectID);
        
        if (!projectExists)
        {
            return;
        }

        Project? project = await context.Projects
            .Where(f => f.ID == projectID)
            .SingleAsync();
        
        context.Projects.Remove(project);
    }

    public static async Task DeleteTask(AppDbContext context, int taskID)
    {
        bool taskExists = await context.Tasks
            .AnyAsync(f => f.ID == taskID);
        
        if (!taskExists)
        {
            return;
        }

        TaskItem? task = await context.Tasks
            .Where(f => f.ID == taskID)
            .SingleAsync();
        
        context.Tasks.Remove(task);
    }

    public static async Task DeleteTag(AppDbContext context, int tagID)
    {
        bool tagExists = await context.Tags
            .AnyAsync(f => f.ID == tagID);
        
        if (!tagExists)
        {
            return;
        }

        Tag? tag = await context.Tags
            .Where(f => f.ID == tagID)
            .SingleAsync();
        
        context.Tags.Remove(tag);
    }

    public static async Task<int> CopyProject(AppDbContext context, int newOwnerID, int projectID, string? newName = null, int? remoteID = null)
    {
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                var project = await GetProjectByID(context, projectID);
                
                if (newName is null)
                {
                    newName = project.Name;

                    while (remoteID is null)
                    {
                        bool isDuplicate = await IsProjectNameDuplicate(context, newOwnerID, newName);

                        if (!isDuplicate)
                        {
                            break;
                        }

                        newName += " - Copy";
                    }
                }

                Project newProject = new(null, remoteID, newOwnerID, newOwnerID, newName, project.Description, project.Status, null, null);

                if (project.Status == ProjectStatus.Completed)
                {
                    newProject.Complete();
                }

                context.Projects.Add(newProject);
                await context.SaveChangesAsync();

                List<TaskItem> tasks = await GetTasksByProject(context, projectID);
                List<TaskItem> newTasks = [];
                Dictionary<int, TaskItem> newTasksByOldID = [];

                foreach (var t in tasks)
                {
                    TaskItem newTask = new(null, null, newProject.ID!.Value, t.Name, t.Description, t.Status, t.Priority, t.DueDate, null, null);

                    if (t.Status == TaskItemStatus.Completed)
                    {
                        newTask.Complete();
                    }

                    newTasks.Add(newTask);
                    newTasksByOldID.Add(t.ID!.Value, newTask);
                }

                List<Tag> tags = await GetTagsByProject(context, projectID);
                List<Tag> newTags = [];
                Dictionary<int, Tag> newTagsByOldID = [];

                foreach (var t in tags)
                {
                    Tag newTag = new(null, null, newProject.ID!.Value, t.Name);
                    newTags.Add(newTag);
                    newTagsByOldID.Add(t.ID!.Value, newTag);
                }

                context.Tasks.AddRange(newTasks);
                context.Tags.AddRange(newTags);
                await context.SaveChangesAsync();

                List<TaskTagLink> links = await context.TaskTagLinks
                    .Where(ttl => newTasksByOldID.Keys.Contains(ttl.TaskID) && newTagsByOldID.Keys.Contains(ttl.TagID))
                    .ToListAsync();
                List<TaskTagLink> newLinks = [];
                
                foreach (var link in links)
                {
                    int? newTaskID = newTasksByOldID[link.TaskID].ID;
                    int? newTagID = newTagsByOldID[link.TagID].ID;

                    newLinks.Add(new(newTaskID!.Value, newTagID!.Value));
                }

                context.TaskTagLinks.AddRange(newLinks);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return newProject.ID!.Value;
            }

            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    public static async Task<Change?> GetChange(AppDbContext context, DomainEntityType targetType, int targetID)
    {
        return await context.Changes
            .SingleOrDefaultAsync(c => c.TargetType == targetType && c.TargetID == targetID);
    }

    public static async Task StoreOrReplaceChange(AppDbContext context, Change change)
    {
        var oldChange = await context.Changes
            .SingleOrDefaultAsync(c => c.UserID == change.UserID && c.TargetType == change.TargetType && c.TargetID == change.TargetID);
        
        if (oldChange is not null)
        {
            oldChange.Name = change.Name;
            oldChange.Description = change.Description;
            oldChange.Status = change.Status;
        }

        else
        {
            context.Changes.Add(change);
        }
    }

    public static async Task DeleteStoredChange(AppDbContext context, int userID, DomainEntityType targetType, int targetID)
    {
        var change = await context.Changes
            .SingleOrDefaultAsync(c => c.UserID == userID && c.TargetType == targetType && c.TargetID == targetID);
        
        if (change is null)
        {
            return;
        }

        else
        {
            context.Changes.Remove(change);
        }
    }
}
