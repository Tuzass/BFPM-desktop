using System.Linq;
using System.Threading.Tasks;
using BFPM.CLI.Pages;
using Services;
using Services.Exceptions;
using BFPM.CLI.Exceptions;

namespace BFPM.CLI.Commands;

public class NewTaskCommand : ICommand
{
    public int ProjectID {get; private set;}

    public NewTaskCommand(int projectID)
    {
        ProjectID = projectID;
    }

    private NewTaskCommand()
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

        var task = await TaskCreationPage.Run(ProjectID);
        // await DomainService.CreateTask(ProjectID, task.Name, task.Description, task.Status, task.Priority, task.DueDate);
    }
}
