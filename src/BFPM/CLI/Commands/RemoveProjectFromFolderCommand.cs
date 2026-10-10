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

        if (!await DomainService.IsFolderOwnedByUser(currentUser.ID, FolderID))
        {
            throw new UnauthorizedAccessException();
        }

        if (!await DomainService.IsUserAssignedToProject(currentUser.ID, ProjectID))
        {
            throw new UnauthorizedAccessException();
        }

        await DomainService.RemoveProjectFromFolder(FolderID, ProjectID);
    }
}
