using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Database;
using Services;

namespace BFPM.CLI.Commands;

public class AssignProjectToFolderCommand : ICommand
{
    public int FolderID {get; private set;}
    public int ProjectID {get; private set;}

    public AssignProjectToFolderCommand(int folderID, int projectID)
    {
        FolderID = folderID;
        ProjectID = projectID;
    }

    private AssignProjectToFolderCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        using (var context = AppDbContextFactory.CreateContext())
        {
            var folders = await DomainService.GetFoldersOwnedByUser(context, currentUser.ID);
            var folder = folders.SingleOrDefault(f => f.ID == FolderID);

            var projects = await DomainService.GetProjectsAssignedToUser(context, currentUser.ID);
            var project = projects.SingleOrDefault(p => p.ID == ProjectID);

            await DomainService.AssignProjectToFolder(context, FolderID, ProjectID);
            await context.SaveChangesAsync();
        }
    }
}
