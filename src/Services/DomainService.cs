using Domain.Entities;
using Domain.Enums;
using Infrastructure.Database;
using Infrastructure.Entities;
using Infrastructure.Enums;
using Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Services;

static public class DomainService
{
    public static async Task<bool> IsFolderOwnedByUser(int userID, int folderID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Folders
                .AnyAsync(f => f.ID == folderID && f.OwnerID == userID);
        }
    }

    public static async Task<bool> IsUserAssignedToProject(int userID, int projectID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Projects
                .AnyAsync(p => p.ID == projectID && p.AssignedUserID == userID);
        }
    }

    public static async Task<bool> IsFolderDuplicate(int ownerID, string folderName)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Folders
                .AnyAsync(f => f.OwnerID == ownerID && f.Name == folderName);
        }
    }

    public static async Task<bool> IsProjectDuplicate(int ownerID, string projectName)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Projects
                .AnyAsync(p => p.OwnerID == ownerID && p.Name == projectName);
        }
    }

    public static async Task<bool> IsTaskDuplicate(int projectID, string taskName)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Tasks
                .AnyAsync(t => t.ProjectID == projectID && t.Name == taskName);
        }
    }

    public static async Task<bool> IsTagDuplicate(int projectID, string tagName)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Tags
                .AnyAsync(t => t.ProjectID == projectID && t.Name == tagName);
        }
    }

    public static async Task<Folder?> GetFolderByID(int folderID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Folders
                .Where(f => f.ID == folderID)
                .SingleOrDefaultAsync();
        }
    }

    public static async Task<Project?> GetProjectByID(int projectID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {   
            return await context.Projects
                .Where(p => p.ID == projectID)
                .SingleOrDefaultAsync();
        }
    }

    public static async Task<TaskItem?> GetTaskByID(int taskID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {   
            return await context.Tasks
                .Where(t => t.ID == taskID)
                .SingleOrDefaultAsync();
        }
    }

    public static async Task<Tag?> GetTagByID(int tagID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {   
            Tag? tag = await context.Tags
                .Where(t => t.ID == tagID)
                .SingleOrDefaultAsync();
            
            if (tag is null)
            {
                throw new NullReferenceException();
            }

            return tag;
        }
    }

    public static async Task<List<User>> GetProjectMembers(int projectID)
    {
        // retrieves a list of all users assigned to a given project, regardless of roles
        // TODO: implement after ApiClient is done
        return [];
    }

    public static async Task<List<User>> GetUsersAssignedToTask(int taskID)
    {
        // retrieves a list of all users assigned to a given task
        // TODO: implement after ApiClient is done
        return [];
    }

    public static async Task<List<Folder>> GetFoldersByProject(int projectID)
    {
        using (var context = AppDbContextFactory.CreateContext())
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
    }

    public static async Task<List<Folder>> GetFoldersByName(int ownerID, string folderName)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Folders
                .Where(f => f.OwnerID == ownerID && f.Name.Contains(folderName))
                .ToListAsync();
        }
    }

    public static async Task<List<Folder>> GetFoldersByUser(int userID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Folders
                .Where(f => f.OwnerID == userID)
                .ToListAsync();
        }
    }

    public static async Task<List<Project>> GetProjectsByFolder(int folderID)
    {
        using (var context = AppDbContextFactory.CreateContext())
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
    }

    public static async Task<List<Project>> GetProjectsByUser(int userID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Projects
                .Where(p => p.AssignedUserID == userID)
                .ToListAsync();
        }
    }

    public static async Task<List<Project>> GetUserProjectsNotAssignedToFolder(int userID, int folderID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Projects
                .Where(p => p.AssignedUserID == userID)
                .Where(p => !context.FolderProjectLinks
                    .Any(l => l.FolderID == folderID && l.ProjectID == p.ID))
                .ToListAsync();
        }
    }

    public static async Task<List<TaskItem>> GetTasksByProject(int projectID)
    {
        Project? project = await GetProjectByID(projectID);
        
        if (project is null)
        {
            throw new InexistentProjectException();
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Tasks
                .Where(t => t.ProjectID == projectID)
                .ToListAsync();
        }
    }

    public static async Task<List<TaskItem>> GetTasksByTag(int tagID)
    {
        Tag? tag = await GetTagByID(tagID);

        if (tag is null)
        {
            throw new InexistentTagException();
        }

        using (var context = AppDbContextFactory.CreateContext())
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
    }

    public static async Task<List<TaskItem>> GetTasksByStatus(int projectID, TaskItemStatus taskStatus)
    {
        Project? project = await GetProjectByID(projectID);

        if (project is null)
        {
            throw new InexistentProjectException();
        }
        
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Tasks
                .Where(t => t.ProjectID == projectID && t.Status == taskStatus)
                .ToListAsync();
        }
    }

    public static async Task<List<TaskItem>> GetTasksByPriority(int projectID, TaskItemPriority taskPriority)
    {
        Project? project = await GetProjectByID(projectID);

        if (project is null)
        {
            throw new InexistentProjectException();
        }
        
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Tasks
                .Where(t => t.ProjectID == projectID && t.Priority == taskPriority)
                .ToListAsync();
        }
    }

    public static async Task<List<TaskItem>> GetTasksByName(int projectID, string taskName)
    {
        Project? project = await GetProjectByID(projectID);

        if (project is null)
        {
            throw new InexistentProjectException();
        }
        
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Tasks
                .Where(t => t.ProjectID == projectID && t.Name.Contains(taskName))
                .ToListAsync();
        }
    }

    public static async Task<List<TaskItem>> GetTasksAssignedToUser(int projectID, int userID)
    {
        // retrieves the tasks on a given project that are assigned to a given user
        // TODO: implement after ApiClient is done
        return [];
    }

    public static async Task<List<Tag>> GetTagsByProject(int projectID)
    {
        Project? project = await GetProjectByID(projectID);

        if (project is null)
        {
            throw new InexistentProjectException();
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Tags
                .Where(t => t.ProjectID == projectID)
                .ToListAsync();
        }
    }

    public static async Task<List<Tag>> GetTagsByTask(int taskID)
    {
        TaskItem? task = await GetTaskByID(taskID);

        if (task is null)
        {
            throw new InexistentTaskException();
        }

        using (var context = AppDbContextFactory.CreateContext())
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
    }

    public static async Task<List<Tag>> GetTagsByName(int projectID, string tagName)
    {
        Project? project = await GetProjectByID(projectID);

        if (project is null)
        {
            throw new InexistentProjectException();
        }
        
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Tags
                .Where(t => t.ProjectID == projectID && t.Name.Contains(tagName))
                .ToListAsync();
        }
    }

    public static async Task CreateFolder(int ownerID, string folderName, string folderDescription)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            bool isDuplicate = await context.Folders
                .AnyAsync(f => f.OwnerID == ownerID && f.Name == folderName);
            
            if (isDuplicate)
            {
                throw new DuplicateFolderException();
            }

            Folder newFolder = new(null, ownerID, folderName, folderDescription);
            context.Folders.Add(newFolder);
            await context.SaveChangesAsync();
        }
    }

    public static async Task CreateProject(int ownerID, string projectName, string projectDescription, ProjectStatus projectStatus)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            bool isDuplicate = await context.Projects
                .AnyAsync(p => p.OwnerID == ownerID && p.Name == projectName);
            
            if (isDuplicate)
            {
                throw new DuplicateProjectException();
            }

            Project newProject = new(null, null, ownerID, ownerID, projectName, projectDescription, projectStatus, null, null);
            context.Projects.Add(newProject);
            await context.SaveChangesAsync();
        }
    }

    public static async Task CreateTask(int projectID, string taskName, string taskDescription, TaskItemStatus taskStatus, TaskItemPriority taskPriority, DateTime? taskDueDate)
    {
        Project? project = await GetProjectByID(projectID);
        
        if (project is null)
        {
            throw new InexistentProjectException();
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            bool isDuplicate = await context.Tasks
                .AnyAsync(t => t.ProjectID == projectID && t.Name == taskName);
            
            if (isDuplicate)
            {
                throw new DuplicateTaskException();
            }

            TaskItem newTask = new(null, projectID, taskName, taskDescription, taskStatus, taskPriority, taskDueDate, null, null);
            context.Tasks.Add(newTask);
            await context.SaveChangesAsync();
        }
    }

    public static async Task CreateTag(int projectID, string tagName)
    {
        Project? project = await GetProjectByID(projectID);

        if (project is null)
        {
            throw new InexistentProjectException();
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            bool isDuplicate = await context.Tags
                .AnyAsync(t => t.ProjectID == projectID && t.Name == tagName);
            
            if (isDuplicate)
            {
                throw new DuplicateTagException();
            }

            Tag newTag = new(null, projectID, tagName);
            context.Tags.Add(newTag);
            await context.SaveChangesAsync();
        }
    }

    public static async Task AssignProjectToFolder(int folderID, int projectID)
    {
        Folder? folder = await GetFolderByID(folderID);
        
        if (folder is null)
        {
            throw new InexistentFolderException();
        }

        Project? project = await GetProjectByID(projectID);

        if (project is null)
        {
            throw new InexistentProjectException();
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            bool isDuplicate = await context.FolderProjectLinks
                .AnyAsync(l => l.FolderID == folderID && l.ProjectID == projectID);
            
            if (!isDuplicate)
            {
                FolderProjectLink newLink = new(folderID, projectID);
                context.FolderProjectLinks.Add(newLink);
                await context.SaveChangesAsync();
            }
        }
    }

    public static async Task RemoveProjectFromFolder(int folderID, int projectID)
    {
        Folder? folder = await GetFolderByID(folderID);
        
        if (folder is null)
        {
            throw new InexistentFolderException();
        }

        Project? project = await GetProjectByID(projectID);

        if (project is null)
        {
            throw new InexistentProjectException();
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            bool linkExists = await context.FolderProjectLinks
                .AnyAsync(l => l.FolderID == folderID && l.ProjectID == projectID);
            
            if (linkExists)
            {
                FolderProjectLink link = new(folderID, projectID);
                context.FolderProjectLinks.Remove(link);
                await context.SaveChangesAsync();
            }
        }
    }

    public static async Task AssignTagToTask(int taskID, int tagID)
    {
        TaskItem? task = await GetTaskByID(taskID);

        if (task is null)
        {
            throw new InexistentTaskException();
        }

        Tag? tag = await GetTagByID(tagID);

        if (tag is null)
        {
            throw new InexistentTagException();
        }

        if (tag.ProjectID != task.ProjectID)
        {
            throw new NonmatchingTaskTagException();
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            bool isDuplicate = await context.TaskTagLinks
                .AnyAsync(l => l.TaskID == taskID && l.TagID == tagID);

            if (!isDuplicate)
            {
                TaskTagLink newLink = new(taskID, tagID);
                context.TaskTagLinks.Add(newLink);
                await context.SaveChangesAsync();
            }
        }
    }

    public static async Task RemoveTagFromTask(int taskID, int tagID)
    {
        TaskItem? task = await GetTaskByID(taskID);
        
        if (task is null)
        {
            throw new InexistentTaskException();
        }

        Tag? tag = await GetTagByID(tagID);

        if (tag is null)
        {
            throw new InexistentTagException();
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            bool linkExists = await context.TaskTagLinks
                .AnyAsync(l => l.TaskID == taskID && l.TagID == tagID);
            
            if (linkExists)
            {
                TaskTagLink link = new(taskID, tagID);
                context.TaskTagLinks.Remove(link);
                await context.SaveChangesAsync();
            }
        }
    }

    public static async Task ApplyLocalChange(LocalChange localChange)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            switch (localChange.TargetType)
            {
                case DomainEntityType.Folder:
                    Folder? folder = await GetFolderByID(localChange.TargetID);

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

                    folder.IncreaseVersion();
                    break;
                
                case DomainEntityType.Project:
                    Project? project = await GetProjectByID(localChange.TargetID);

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

                    project.IncreaseVersion();
                    break;
                
                case DomainEntityType.Task:
                    TaskItem? task = await GetTaskByID(localChange.TargetID);

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

                    if (localChange.DueDate is not null)
                    {
                        task.ChangeDueDate((DateTime)localChange.DueDate);
                    }

                    task.IncreaseVersion();
                    break;
                
                case DomainEntityType.Tag:
                    Tag? tag = await GetTagByID(localChange.TargetID);

                    if (tag is null)
                    {
                        throw new InexistentTagException();
                    }

                    if (localChange.Name is not null)
                    {
                        tag.Name = localChange.Name;
                    }

                    tag.IncreaseVersion();
                    break;
            }

            await context.SaveChangesAsync();
        }
    }

    public static async Task DeleteFolder(int folderID)
    {
        using (var context = AppDbContextFactory.CreateContext())
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
            await context.SaveChangesAsync();
        }
    }

    public static async Task DeleteProject(int projectID)
    {
        using (var context = AppDbContextFactory.CreateContext())
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
            await context.SaveChangesAsync();
        }
    }

    public static async Task DeleteTask(int taskID)
    {
        using (var context = AppDbContextFactory.CreateContext())
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
            await context.SaveChangesAsync();
        }
    }

    public static async Task DeleteTag(int tagID)
    {
        using (var context = AppDbContextFactory.CreateContext())
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
            await context.SaveChangesAsync();
        }
    }

    public static async Task CopyProject(int newOwnerID, int projectID, int? remoteID = null)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            Project? project = await context.Projects
                .Where(p => p.ID == projectID)
                .SingleOrDefaultAsync();
            
            if (project is null)
            {
                throw new InexistentProjectException();
            }

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

            if (newProject.ID is null)
            {
                throw new NullReferenceException();
            }

            List<TaskItem> tasks = await GetTasksByProject(projectID);
            List<TaskItem> newTasks = [];
            Dictionary<int, TaskItem> newTasksByOldID = [];

            foreach (var t in tasks)
            {
                if (t.ID is null)
                {
                    throw new NullReferenceException();
                }

                TaskItem newTask = new(null, newProject.ID.Value, t.Name, t.Description, t.Status, t.Priority, t.DueDate, null, null);

                if (t.Status == TaskItemStatus.Completed)
                {
                    newTask.Complete();
                }

                newTasks.Add(newTask);
                newTasksByOldID.Add(t.ID.Value, newTask);
            }

            List<Tag> tags = await GetTagsByProject(projectID);
            List<Tag> newTags = [];
            Dictionary<int, Tag> newTagsByOldID = [];

            foreach (var t in tags)
            {
                if (t.ID is null)
                {
                    throw new NullReferenceException();
                }

                Tag newTag = new(null, newProject.ID.Value, t.Name);
                newTags.Add(newTag);
                newTagsByOldID.Add(t.ID.Value, newTag);
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

                if (newTaskID is null || newTagID is null)
                {
                    throw new NullReferenceException();
                }

                newLinks.Add(new(newTaskID.Value, newTagID.Value));
            }

            context.TaskTagLinks.AddRange(newLinks);
            await context.SaveChangesAsync();
        }
    }
}
