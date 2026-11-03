using System;
using System.Threading.Tasks;
using BFPM.CLI.Pages;
using Services;
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

        if (!await DomainService.IsUserAssignedToProject(currentUser.ID, ProjectID))
        {
            throw new Services.Exceptions.UnauthorizedAccessException();
        }

        if (!await DomainService.IsProjectLocal(ProjectID))
        {
            throw new NonlocalChangeException();
        }

        var localChange = await ProjectEditPage.Run(ProjectID);

        if (localChange is null)
        {
            return;
        }

        Console.WriteLine($"Project {ProjectID} changed\nName: {localChange.Name}, Description: {localChange.Description}, Status: {localChange.Status}");
        await DomainService.ApplyChange(localChange);
    }
}
