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
        var projects = await DomainService.GetCurrentUserProjects();
        var project = projects.SingleOrDefault(p => p.ID == ProjectID);

        if (project is null)
        {
            throw new UnauthorizedAccessException();
        }

        if (project.RemoteID is not null)
        {
            throw new NonlocalChangeException();
        }

        var localChange = await ProjectEditPage.Run(project);
        // await DomainService.ApplyLocalChange(localChange);
    }
}
