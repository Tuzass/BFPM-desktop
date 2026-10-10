using System;
using System.Threading.Tasks;
using BFPM.CLI.Exceptions;
using BFPM.CLI.Pages;
using Services;

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

        if (!await DomainService.IsUserAssignedToProject(currentUser.ID, ProjectID))
        {
            throw new Services.Exceptions.UnauthorizedAccessException();
        }

        if (!await DomainService.IsProjectLocal(ProjectID))
        {
            throw new NonlocalChangeException();
        }

        var task = await TagCreationPage.Run(ProjectID);

        if (task is null)
        {
            return;
        }

        Console.WriteLine($"New tag created\nName: {task.Name}");
        await DomainService.CreateTag(ProjectID, task.Name);
    }
}
