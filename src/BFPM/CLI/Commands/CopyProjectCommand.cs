using System;
using System.Threading.Tasks;
using Services;

namespace BFPM.CLI.Commands;

public class CopyProjectCommand : ICommand
{
    public int ProjectID {get; private set;}

    public CopyProjectCommand(int projectID)
    {
        ProjectID = projectID;
    }

    private CopyProjectCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        if (await DomainService.IsUserAssignedToProject(currentUser.ID, ProjectID))
        {
            await DomainService.CopyProject(currentUser.ID, ProjectID);
        }

        throw new UnauthorizedAccessException();
    }
}
