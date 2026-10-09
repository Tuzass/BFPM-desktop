using System;
using System.Linq;
using System.Threading.Tasks;
using Services;

namespace BFPM.CLI.Commands;

public class PrintTaskCommand : ICommand
{
    public int TaskID {get; private set;}

    public PrintTaskCommand(int taskID)
    {
        TaskID = taskID;
    }

    private PrintTaskCommand()
    {
        
    }

    public async Task Execute()
    {
        var task = await DomainService.GetTaskByID(TaskID);

        var currentUser = await UserService.GetCurrentUser();

        if (!await DomainService.IsUserAssignedToProject(currentUser.ID, task.ProjectID))
        {
            throw new UnauthorizedAccessException();
        }

        Console.WriteLine(Utils.TaskToString(task));
    }
}
