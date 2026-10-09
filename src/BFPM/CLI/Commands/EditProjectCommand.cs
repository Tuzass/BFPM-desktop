using System.Linq;
using System.Threading.Tasks;
using BFPM.CLI.Pages;
using Services;
using Services.Exceptions;
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
            throw new UnauthorizedAccessException();
        }

        if (!await DomainService.IsProjectLocal(ProjectID))
        {
            throw new NonlocalChangeException();
        }

        var project = await DomainService.GetProjectByID(ProjectID);
        var localChange = await ProjectEditPage.Run(project);
        // await DomainService.ApplyLocalChange(localChange);
    }
}
