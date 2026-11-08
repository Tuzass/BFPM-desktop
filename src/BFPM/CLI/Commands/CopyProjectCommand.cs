using System;
using System.Threading.Tasks;
using Infrastructure.Database;
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

        using (var context = AppDbContextFactory.CreateContext())
        {
            if (!await DomainService.IsProjectAssignedToUser(context, currentUser.ID, ProjectID))
            {
                throw new UnauthorizedAccessException();
            }

            await DomainService.CopyProject(context, currentUser.ID, ProjectID);
            await context.SaveChangesAsync();
        }
    }
}
