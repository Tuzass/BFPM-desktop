using System.Threading.Tasks;
using Infrastructure.Database;
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

        using (var context = AppDbContextFactory.CreateContext())
        {
            if (!await DomainService.IsProjectAssignedToUser(context, currentUser.ID, ProjectID))
            {
                // TODO: change to IsProjectOwnerByUser
                throw new UnauthorizedAccessException();
            }

            if (!await DomainService.IsProjectLocal(context, ProjectID))
            {
                // TODO: implement remote project deletion if associated with a remote project
                throw new NonlocalProjectException();
            }

            await DomainService.DeleteProject(context, ProjectID);
            await context.SaveChangesAsync();
        }
    }
}
