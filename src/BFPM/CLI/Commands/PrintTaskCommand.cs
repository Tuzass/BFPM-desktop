using System;
using System.Threading.Tasks;
using Infrastructure.Database;
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
        var currentUser = await UserService.GetCurrentUser();

        using (var context = AppDbContextFactory.CreateContext())
        {   
            var task = await DomainService.GetTaskByID(context, TaskID);

            if (!await DomainService.IsProjectAssignedToUser(context, currentUser.ID, task.ProjectID))
            {
                throw new UnauthorizedAccessException();
            }

            var tags = await DomainService.GetTagsByTask(context, TaskID);
            Console.WriteLine(Utils.TaskToString(task));
            Console.WriteLine($"Tags: {Utils.TagListToString(tags)}");
        }
    }
}
