using Domain.Entities;
using Domain.Enums;
using Infrastructure.Database;
using Infrastructure.Entities;
using Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Services;

static public class DomainService
{
    public static async Task<List<Folder>> GetCurrentUserFolders()
    {
        User? currentUser = await UserService.GetCurrentUser();

        if (currentUser is null)
        {
            throw new NoCurrentUserException();
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Folders
                .Where(f => f.OwnerID == currentUser.ID)
                .ToListAsync();
        }
    }

    public static async Task<List<Project>> GetCurrentUserProjects()
    {
        User? currentUser = await UserService.GetCurrentUser();

        if (currentUser is null)
        {
            throw new NoCurrentUserException();
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Projects
                .Where(p => p.OwnerID == currentUser.ID || p.AssignedUserID == currentUser.ID)
                .ToListAsync();
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
        await GetFolderByID(projectID);

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

    public static async Task<List<Folder>> GetFoldersByName(string folderName)
    {
        User? currentUser = await UserService.GetCurrentUser();

        if (currentUser is null)
        {
            throw new NoCurrentUserException();
        }
        
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Folders
                .Where(f => f.OwnerID == currentUser.ID && f.Name.Contains(folderName.ToLowerInvariant()))
                .ToListAsync();
        }
    }

    public static async Task<List<Project>> GetProjectsByFolder(int folderID)
    {
        Folder? folder = await GetFolderByID(folderID);

        if (folder is null)
        {
            throw new InexistentFolderException();
        }

        User? currentUser = await UserService.GetCurrentUser();

        if (currentUser is null)
        {
            throw new NoCurrentUserException();
        }

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
                .Where(t => t.ProjectID == projectID && t.Name.Contains(taskName.ToLowerInvariant()))
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
                .Where(t => t.ProjectID == projectID && t.Name.Contains(tagName.ToLowerInvariant()))
                .ToListAsync();
        }
    }

    public static async Task CreateFolder(string folderName, string folderDescription)
    {
        User? currentUser = await UserService.GetCurrentUser();

        if (currentUser is null)
        {
            throw new NoCurrentUserException();
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            bool isDuplicate = await context.Folders
                .AnyAsync(f => f.OwnerID == currentUser.ID && f.Name == folderName.ToLowerInvariant());
            
            if (isDuplicate)
            {
                throw new DuplicateFolderException();
            }

            Folder newFolder = new(currentUser.ID, folderName.ToLowerInvariant(), folderDescription);
            context.Folders.Add(newFolder);
            await context.SaveChangesAsync();
        }
    }

    public static async Task CreateProject(string projectName, string projectDescription, ProjectStatus projectStatus)
    {
        User? currentUser = await UserService.GetCurrentUser();

        if (currentUser is null)
        {
            throw new NoCurrentUserException();
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            bool isDuplicate = await context.Projects
                .AnyAsync(p => p.OwnerID == currentUser.ID && p.Name == projectName.ToLowerInvariant());
            
            if (isDuplicate)
            {
                throw new DuplicateProjectException();
            }

            Project newProject = new(currentUser.ID, projectName.ToLowerInvariant(), projectDescription, projectStatus, true, null, null);
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
                .AnyAsync(t => t.ProjectID == projectID && t.Name == taskName.ToLowerInvariant());
            
            if (isDuplicate)
            {
                throw new DuplicateTaskException();
            }

            TaskItem newTask = new(projectID, taskName.ToLowerInvariant(), taskDescription, taskStatus, taskPriority, taskDueDate, null, null);
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
                .AnyAsync(t => t.ProjectID == projectID && t.Name == tagName.ToLowerInvariant());
            
            if (isDuplicate)
            {
                throw new DuplicateTagException();
            }

            Tag newTag = new(projectID, tagName.ToLowerInvariant());
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
}
