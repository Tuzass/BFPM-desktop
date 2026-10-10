using System.Linq;
using System.Threading.Tasks;
using BFPM.CLI.Exceptions;
using BFPM.CLI.Pages;
using Services;
using Services.Exceptions;

namespace BFPM.CLI.Commands;

public class EditTaskCommand : ICommand
{
    public int TaskID {get; private set;}

    public EditTaskCommand(int taskID)
    {
        TaskID = taskID;
    }

    private EditTaskCommand()
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

        var localChange = await TaskEditPage.Run(TaskID);

        if (localChange is null)
        {
            return;
        }

        await DomainService.ApplyLocalChange(localChange);
    }
}
