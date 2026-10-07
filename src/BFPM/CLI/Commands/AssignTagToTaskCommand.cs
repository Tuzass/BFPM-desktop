using System.Linq;
using System.Threading.Tasks;
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
        var task = await DomainService.GetTaskByID(TaskID);
        var tag = await DomainService.GetTagByID(TagID);

        if (task is null || tag is null)
        {
            throw new UnauthorizedAccessException();
        }

        var projects = await DomainService.GetCurrentUserProjects();
        var taskProject = projects.SingleOrDefault(p => p.ID == task.ProjectID);
        var tagProject = projects.SingleOrDefault(p => p.ID == tag.ProjectID);

        if (taskProject is null || tagProject is null)
        {
            throw new UnauthorizedAccessException();
        }

        if (task.ProjectID != tag.ProjectID)
        {
            throw new NonmatchingTaskTagException();
        }

        if (taskProject.RemoteID is not null)
        {
            throw new NonlocalChangeException();
        }

        // await DomainService.AssignTagToTask(TaskID, TagID);
    }
}
