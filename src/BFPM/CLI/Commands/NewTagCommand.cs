using System.Linq;
using System.Threading.Tasks;
using BFPM.CLI.Exceptions;
using BFPM.CLI.Pages;
using Services;
using Services.Exceptions;

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

        var task = await TagCreationPage.Run(ProjectID);
        // await DomainService.CreateTag(ProjectID, task.Name);
    }
}
