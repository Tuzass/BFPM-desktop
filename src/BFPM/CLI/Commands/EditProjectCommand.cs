using System;
using System.Threading.Tasks;
using Services;
using Infrastructure.Database;
using BFPM.CLI.Pages;
using BFPM.CLI.Exceptions;

namespace BFPM.CLI.Commands;

public class EditProjectCommand : ICommand
{
    public int ProjectID {get; private set;}

    public EditProjectCommand(int projectID)
    {
        ProjectID = projectID;
    }

    private EditProjectCommand()
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

            var localChange = await ProjectEditPage.Run(context, ProjectID);

            if (localChange is null)
            {
                return;
            }
            
            await DomainService.ApplyChange(context, localChange);
            await context.SaveChangesAsync();

            Console.WriteLine($"Project {ProjectID} changed\nName: {localChange.Name}, Description: {localChange.Description}, Status: {localChange.Status}");
        }
    }
}
