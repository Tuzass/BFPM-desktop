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
        var folders = await DomainService.GetCurrentUserFolders();
        var folder = folders.SingleOrDefault(f => f.ID == FolderID);

        if (folder is null)
        {
            throw new UnauthorizedAccessException();
        }

        var projects = await DomainService.GetCurrentUserProjects();
        var project = projects.SingleOrDefault(p => p.ID == ProjectID);

        if (project is null)
        {
            throw new UnauthorizedAccessException();
        }

        // await DomainService.AssignProjectToFolder(FolderID, ProjectID);
    }
}
