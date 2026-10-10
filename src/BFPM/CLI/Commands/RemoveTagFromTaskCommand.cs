using System.Linq;
using System.Threading.Tasks;
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

        var task = await DomainService.GetTaskByID(TaskID);
        var tag = await DomainService.GetTagByID(TagID);

        if (!await DomainService.IsUserAssignedToProject(currentUser.ID, task.ProjectID)
         || !await DomainService.IsUserAssignedToProject(currentUser.ID, tag.ProjectID))
        {
            throw new UnauthorizedAccessException();
        }

        if (task.ProjectID != tag.ProjectID)
        {
            throw new NonmatchingTaskTagException();
        }

        if (!await DomainService.IsProjectLocal(task.ProjectID))
        {
            throw new NonlocalChangeException();
        }

        await DomainService.RemoveTagFromTask(TaskID, TagID);
    }
}
