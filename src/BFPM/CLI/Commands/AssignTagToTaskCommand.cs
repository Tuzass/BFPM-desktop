using System.Threading.Tasks;
using Infrastructure.Database;
using Services;
using Services.Exceptions;
using BFPM.CLI.Exceptions;

namespace BFPM.CLI.Commands;

public class AssignTagToTaskCommand : ICommand
{
    public int TaskID {get; private set;}
    public int TagID {get; private set;}

    public AssignTagToTaskCommand(int taskID, int tagID)
    {
        TaskID = taskID;
        TagID = tagID;
    }

    private AssignTagToTaskCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        using (var context = AppDbContextFactory.CreateContext())
        {
            var task = await DomainService.GetTaskByID(context, TaskID);
            var tag = await DomainService.GetTagByID(context, TagID);

            if (!await DomainService.IsProjectAssignedToUser(context, currentUser.ID, task.ProjectID)
            || !await DomainService.IsProjectAssignedToUser(context, currentUser.ID, tag.ProjectID))
            {
                throw new UnauthorizedAccessException();
            }

            if (task.ProjectID != tag.ProjectID)
            {
                throw new NonmatchingTaskTagException();
            }

            if (!await DomainService.IsProjectLocal(context, task.ProjectID))
            {
                throw new NonlocalProjectException();
            }

            await DomainService.AssignTagToTask(context, TaskID, TagID);
            await context.SaveChangesAsync();
        }
    }
}
