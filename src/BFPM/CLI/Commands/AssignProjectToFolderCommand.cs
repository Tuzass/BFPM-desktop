using System.Linq;
using System.Threading.Tasks;
using Services;
using Services.Exceptions;

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

        var folders = await DomainService.GetFoldersByUser(currentUser.ID);
        var folder = folders.SingleOrDefault(f => f.ID == FolderID);

        var projects = await DomainService.GetProjectsByUser(currentUser.ID);
        var project = projects.SingleOrDefault(p => p.ID == ProjectID);

        await DomainService.AssignProjectToFolder(FolderID, ProjectID);
    }
}
