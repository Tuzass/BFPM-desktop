using System.Linq;
using System.Threading.Tasks;
using Services;
using Services.Exceptions;
using BFPM.CLI.Exceptions;

namespace BFPM.CLI.Commands;

public class DeleteTaskCommand : ICommand
{
    public int TaskID {get; private set;}

    public DeleteTaskCommand(int taskID)
    {
        TaskID = taskID;
    }

    private DeleteTaskCommand()
    {
        
    }

    public async Task Execute()
    {
        var task = await DomainService.GetTaskByID(TaskID);

        if (task is null)
        {
            throw new UnauthorizedAccessException();
        }

        var currentUser = await UserService.GetCurrentUser();

        var projects = await DomainService.GetProjectsByUser(currentUser.ID);
        var project = projects.SingleOrDefault(p => p.ID == task.ProjectID);

        if (project is null)
        {
            throw new UnauthorizedAccessException();
        }

        if (project.RemoteID is not null)
        {
            throw new NonlocalChangeException();
        }
        
        // await DomainService.DeleteTask(TaskID);
    }
}
