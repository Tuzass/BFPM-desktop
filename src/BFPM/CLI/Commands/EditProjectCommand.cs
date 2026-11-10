using System;
using System.Threading.Tasks;
using Services;
using Infrastructure.Database;
using BFPM.CLI.Pages;
using BFPM.CLI.Exceptions;

namespace BFPM.CLI.Commands;

public class EnterProjectCommand : ICommand
{
    public int ProjectID {get; private set;}

    public EnterProjectCommand(int projectID)
    {
        ProjectID = projectID;
    }

    private EnterProjectCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        using (var context = AppDbContextFactory.CreateContext())
        {
            if (!await DomainService.IsProjectAssignedToUser(context, currentUser.ID, ProjectID))
            {
                throw new Services.Exceptions.UnauthorizedAccessException();
            }

            if (!await DomainService.IsProjectLocal(context, ProjectID))
            {
                throw new NonlocalChangeException();
            }

            var change = await ProjectEditPage.Run(context, ProjectID);

            if (change is null)
            {
                return;
            }
            
            if (change.Name is not null || change.Description is not null || change.Status is not null)
            {
                await DomainService.ApplyChange(context, change);
                await context.SaveChangesAsync();

                Console.WriteLine($"Project {ProjectID} changed");
            }
        }
    }
}
