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

    public static async Task<bool> IsUserAssignedToProject(AppDbContext context, int userID, int projectID)
    {
        return await context.Projects
            .AnyAsync(p => p.ID == projectID && p.AssignedUserID == userID);
    }

    public static async Task<bool> IsFolderDuplicate(AppDbContext context, int ownerID, string folderName, int? folderID = null)
    {
        return await context.Folders
            .AnyAsync(f => f.ID != folderID && f.OwnerID == ownerID && f.Name == folderName);
    }

    public static async Task<bool> IsProjectDuplicate(AppDbContext context, int ownerID, string projectName, int? projectID = null)
    {
        return await context.Projects
            .AnyAsync(p => p.ID != projectID && p.OwnerID == ownerID && p.Name == projectName);
    }

    public static async Task<bool> IsProjectLocal(AppDbContext context, int projectID)
    {
        var project = await GetProjectByID(context, projectID);
        return project.RemoteID is null;
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

    public static async Task<List<User>> GetProjectMembers(AppDbContext context, int projectID)
    {
        // retrieves a list of all users assigned to a given project, regardless of roles
        // TODO: implement after ApiClient is done
        return [];
    }

    public static async Task<List<User>> GetUsersAssignedToTask(AppDbContext context, int taskID)
    {
        // retrieves a list of all users assigned to a given task
        // TODO: implement after ApiClient is done
        return [];
    }

    public static async Task<List<Folder>> GetFoldersByProject(AppDbContext context, int projectID)
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

    public static async Task<List<Folder>> GetFoldersByUser(AppDbContext context, int userID)
    {
        return await context.Folders
            .Where(f => f.OwnerID == userID)
            .ToListAsync();
    }

    public static async Task<List<Project>> GetProjectsByFolder(AppDbContext context, int folderID)
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

    public static async Task<List<Project>> GetProjectsByUser(AppDbContext context, int userID)
    {
        return await context.Projects
            .Where(p => p.AssignedUserID == userID)
            .ToListAsync();
    }

    public static async Task<List<Project>> GetUserProjectsNotAssignedToFolder(AppDbContext context, int userID, int folderID)
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

    public static async Task<List<TaskItem>> GetTasksAssignedToUser(AppDbContext context, int projectID, int userID)
    {
        // retrieves the tasks on a given project that are assigned to a given user
        // TODO: implement after ApiClient is done
        return [];
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

        TaskItem newTask = new(null, projectID, taskName, taskDescription, taskStatus, taskPriority, taskDueDate, null, null);
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

        Tag newTag = new(null, projectID, tagName);
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

    public static async Task ApplyChange(AppDbContext context, Change localChange)
    {
        switch (localChange.TargetType)
        {
            case DomainEntityType.Folder:
                var folder = await context.Folders
                    .SingleOrDefaultAsync(f => f.ID == localChange.TargetID);
                
                if (folder is null)
                {
                    throw new InexistentFolderException();
                }

                if (localChange.Name is not null)
                {
                    folder.Name = localChange.Name;
                }

                if (localChange.Description is not null)
                {
                    folder.Description = localChange.Description;
                }

                if (localChange.Name is not null
                    || localChange.Description is not null)
                {
                    folder.IncreaseVersion();                        
                }

                break;
            
            case DomainEntityType.Project:
                var project = await context.Projects
                    .SingleOrDefaultAsync(f => f.ID == localChange.TargetID);
                
                if (project is null)
                {
                    throw new InexistentProjectException();
                }

                if (localChange.Name is not null)
                {
                    project.Name = localChange.Name;
                }

                if (localChange.Description is not null)
                {
                    project.Description = localChange.Description;
                }

                if (localChange.Status is not null)
                {
                    switch (localChange.Status)
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
                    }
                }

                if (localChange.Name is not null
                    || localChange.Description is not null
                    || localChange.Status is not null)
                {
                    project.IncreaseVersion();                        
                }

                break;
            
            case DomainEntityType.Task:
                var task = await context.Tasks
                    .SingleOrDefaultAsync(f => f.ID == localChange.TargetID);
                
                if (task is null)
                {
                    throw new InexistentTaskException();
                }

                if (localChange.Name is not null)
                {
                    task.Name = localChange.Name;
                }

                if (localChange.Description is not null)
                {
                    task.Description = localChange.Description;
                }

                if (localChange.Status is not null)
                {
                    switch (localChange.Status)
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
                    }
                }

                if (localChange.Priority is not null)
                {
                    switch (localChange.Priority)
                    {
                        case TaskItemPriority.Low:
                            task.Priority = TaskItemPriority.Low;
                            break;

                        case TaskItemPriority.Medium:
                            task.Priority = TaskItemPriority.Medium;
                            break;
                        
                        case TaskItemPriority.High:
                            task.Priority = TaskItemPriority.High;
                            break;
                        
                        case TaskItemPriority.Urgent:
                            task.Priority = TaskItemPriority.Urgent;
                            break;
                    }
                }

                if (localChange.DueDateChanged)
                {
                    task.ChangeDueDate(localChange.DueDate);
                }

                if (localChange.Name is not null
                    || localChange.Description is not null
                    || localChange.Status is not null
                    || localChange.Priority is not null
                    || localChange.DueDateChanged)
                {
                    task.IncreaseVersion();                        
                }

                break;
            
            case DomainEntityType.Tag:
                var tag = await context.Tags
                    .SingleOrDefaultAsync(f => f.ID == localChange.TargetID);
                
                if (tag is null)
                {
                    throw new InexistentTagException();
                }

                if (localChange.Name is not null)
                {
                    tag.Name = localChange.Name;
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

    public static async Task CopyProject(AppDbContext context, int newOwnerID, int projectID, int? remoteID = null)
    {
        var project = await GetProjectByID(context, projectID);

        string newProjectName = project.Name;
        bool isDuplicate = await context.Projects
            .AnyAsync(p => p.OwnerID == newOwnerID && p.Name == project.Name);     

        while (isDuplicate)
        {
            newProjectName += " - copy";
            isDuplicate = await context.Projects
            .AnyAsync(p => p.OwnerID == newOwnerID && p.Name == newProjectName);
        }

        Project newProject = new(null, remoteID, newOwnerID, newOwnerID, newProjectName, project.Description, project.Status, null, null);

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
            TaskItem newTask = new(null, newProject.ID!.Value, t.Name, t.Description, t.Status, t.Priority, t.DueDate, null, null);

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
            Tag newTag = new(null, newProject.ID!.Value, t.Name);
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
    }
}
