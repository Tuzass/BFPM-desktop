using System.Threading.Tasks;
using System.Linq;
using Services;
using Services.Exceptions;

namespace BFPM.CLI.Commands;

public class RemoveProjectFromFolderCommand : ICommand
{
    public int FolderID {get; private set;}
    public int ProjectID {get; private set;}

    public RemoveProjectFromFolderCommand(int folderID, int projectID)
    {
        FolderID = folderID;
        ProjectID = projectID;
    }

    private RemoveProjectFromFolderCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        var folders = await DomainService.GetFoldersByUser(currentUser.ID);
        var folder = folders.SingleOrDefault(f => f.ID == FolderID);

        if (folder is null)
        {
            throw new UnauthorizedAccessException();
        }

        var projects = await DomainService.GetProjectsByUser(currentUser.ID);
        var project = projects.SingleOrDefault(p => p.ID == ProjectID);

        if (project is null)
        {
            throw new UnauthorizedAccessException();
        }

        // await DomainService.RemoveProjectFromFolder(FolderID, ProjectID);
    }
}
