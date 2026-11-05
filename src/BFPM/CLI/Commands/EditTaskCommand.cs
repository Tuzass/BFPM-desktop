using System;
using System.Threading.Tasks;
using Infrastructure.Database;
using Services;
using BFPM.CLI.Exceptions;
using BFPM.CLI.Pages;

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

        using (var context = AppDbContextFactory.CreateContext())
        {
            var task = await DomainService.GetTaskByID(context, TaskID);

            if (!await DomainService.IsUserAssignedToProject(context, currentUser.ID, task.ProjectID))
            {
                throw new UnauthorizedAccessException();
            }

            if (!await DomainService.IsProjectLocal(context, task.ProjectID))
            {
                throw new NonlocalChangeException();
            }

            var localChange = await TaskEditPage.Run(context, TaskID);

            if (localChange is null)
            {
                return;
            }

            await DomainService.ApplyChange(context, localChange);
            await context.SaveChangesAsync();

            Console.WriteLine($"Task {TaskID} changed\nName: {localChange.Name}, Description: {localChange.Description}, Status: {localChange.Status}, Priority: {localChange.Priority}");
        }
    }
}
