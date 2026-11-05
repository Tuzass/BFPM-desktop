using System.Threading.Tasks;
using Infrastructure.Database;
using Services;
using Services.Exceptions;
using BFPM.CLI.Exceptions;

namespace BFPM.CLI.Commands;

public class RemoveTagFromTaskCommand : ICommand
{
    public int TaskID {get; private set;}
    public int TagID {get; private set;}

    public RemoveTagFromTaskCommand(int taskID, int tagID)
    {
        TaskID = taskID;
        TagID = tagID;
    }

    private RemoveTagFromTaskCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        using (var context = AppDbContextFactory.CreateContext())
        {
            var task = await DomainService.GetTaskByID(context, TaskID);
            var tag = await DomainService.GetTagByID(context, TagID);

            if (!await DomainService.IsUserAssignedToProject(context, currentUser.ID, task.ProjectID)
            || !await DomainService.IsUserAssignedToProject(context, currentUser.ID, tag.ProjectID))
            {
                throw new UnauthorizedAccessException();
            }

            if (task.ProjectID != tag.ProjectID)
            {
                throw new NonmatchingTaskTagException();
            }

            if (!await DomainService.IsProjectLocal(context, task.ProjectID))
            {
                throw new NonlocalChangeException();
            }

            await DomainService.RemoveTagFromTask(context, TaskID, TagID);
            await context.SaveChangesAsync();
        }
    }
}
