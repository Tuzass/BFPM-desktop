using System;
using System.Threading.Tasks;
using Infrastructure.Database;
using Services;
using BFPM.CLI.Exceptions;
using BFPM.CLI.Pages;

namespace BFPM.CLI.Commands;

public class NewTagCommand : ICommand
{
    public int ProjectID {get; private set;}

    public NewTagCommand(int projectID)
    {
        ProjectID = projectID;
    }

    private NewTagCommand()
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

            var task = await TagCreationPage.Run(context, ProjectID);

            if (task is null)
            {
                return;
            }

            await DomainService.CreateTag(context, ProjectID, task.Name);
            await context.SaveChangesAsync();
            
            Console.WriteLine($"New tag created\nName: {task.Name}");
        }
    }
}
