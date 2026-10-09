using System;
using System.Linq;
using System.Threading.Tasks;
using Services;

namespace BFPM.CLI.Commands;

public class PrintProjectCommand : ICommand
{
    public int ProjectID {get; private set;}

    public PrintProjectCommand(int projectID)
    {
        ProjectID = projectID;
    }

    private PrintProjectCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        if (!await DomainService.IsUserAssignedToProject(currentUser.ID, ProjectID))
        {
            throw new UnauthorizedAccessException();
        }

        var project = await DomainService.GetProjectByID(ProjectID);
        Console.WriteLine(Utils.ProjectToString(project));
    }
}
