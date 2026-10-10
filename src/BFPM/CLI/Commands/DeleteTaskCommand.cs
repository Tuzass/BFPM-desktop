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
        var currentUser = await UserService.GetCurrentUser();

        var task = await DomainService.GetTaskByID(TaskID);

        if (!await DomainService.IsUserAssignedToProject(currentUser.ID, task.ProjectID))
        {
            throw new UnauthorizedAccessException();
        }

        if (!await DomainService.IsProjectLocal(task.ProjectID))
        {
            throw new NonlocalChangeException();
        }
        
        await DomainService.DeleteTask(TaskID);
    }
}
