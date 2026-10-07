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
        if (await DomainService.IsCurrentUserAssignedToProject(ProjectID))
        {
            await DomainService.CopyProject(ProjectID);
        }

        throw new UnauthorizedAccessException();
    }
}
