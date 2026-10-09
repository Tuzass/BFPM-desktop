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

        var projects = await DomainService.GetProjectsByUser(currentUser.ID);
        var project = projects.SingleOrDefault(p => p.ID == ProjectID);

        if (project is null)
        {
            throw new UnauthorizedAccessException();
        }

        if (project.RemoteID is not null)
        {
            throw new NonlocalChangeException();
        }

        // await DomainService.DeleteProject(ProjectID);
    }
}
