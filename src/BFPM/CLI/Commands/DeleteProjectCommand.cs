using System.Linq;
using System.Threading.Tasks;
using Services;
using Services.Exceptions;
using BFPM.CLI.Exceptions;

namespace BFPM.CLI.Commands;

public class DeleteProjectCommand : ICommand
{
    public int ProjectID {get; private set;}

    public DeleteProjectCommand(int projectID)
    {
        ProjectID = projectID;
    }

    private DeleteProjectCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        if (!await DomainService.IsUserAssignedToProject(currentUser.ID, ProjectID))
        {
            throw new UnauthorizedAccessException();
        }

        if (!await DomainService.IsProjectLocal(ProjectID))
        {
            throw new NonlocalChangeException();
        }

        // await DomainService.DeleteProject(ProjectID);
    }
}
