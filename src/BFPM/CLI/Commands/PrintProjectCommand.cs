using System;
using System.Threading.Tasks;
using Infrastructure.Database;
using Services;

namespace BFPM.CLI.Commands;

public class PrintProjectCommand : ICommand
{
    public int ProjectID {get; private set;}

    public PrintProjectCommand(int projectID)
    {
        ProjectID = projectID;
    }

    private PrintProjectCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        using (var context = AppDbContextFactory.CreateContext())
        {
            if (!await DomainService.IsUserAssignedToProject(context, currentUser.ID, ProjectID))
            {
                throw new UnauthorizedAccessException();
            }

            var project = await DomainService.GetProjectByID(context, ProjectID);
            Console.WriteLine(Utils.ProjectToString(project));

            var tasks = await DomainService.GetTasksByProject(context, ProjectID);
            Console.WriteLine($"Tasks: {Utils.TaskListToString(tasks)}");

            var tags = await DomainService.GetTagsByProject(context, ProjectID);
            Console.WriteLine($"Tags: {Utils.TagListToString(tags)}");
        }
    }
}
