using System.Threading.Tasks;
using Infrastructure.Database;
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

        using (var context = AppDbContextFactory.CreateContext())
        {
            var task = await DomainService.GetTaskByID(context, TaskID);

            if (!await DomainService.IsProjectAssignedToUser(context, currentUser.ID, task.ProjectID))
            {
                throw new UnauthorizedAccessException();
            }

            if (!await DomainService.IsProjectLocal(context, task.ProjectID))
            {
                throw new NonlocalProjectException();
            }
            
            await DomainService.DeleteTask(context, TaskID);
            await context.SaveChangesAsync();
        }
    }
}
