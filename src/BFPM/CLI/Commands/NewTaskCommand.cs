using System;
using System.Threading.Tasks;
using Infrastructure.Database;
using Services;
using BFPM.CLI.Pages;
using BFPM.CLI.Exceptions;

namespace BFPM.CLI.Commands;

public class NewTaskCommand : ICommand
{
    public int ProjectID {get; private set;}

    public NewTaskCommand(int projectID)
    {
        ProjectID = projectID;
    }

    private NewTaskCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        using (var context = AppDbContextFactory.CreateContext())
        {
            if (!await DomainService.IsProjectAssignedToUser(context, currentUser.ID, ProjectID))
            {
                throw new Services.Exceptions.UnauthorizedAccessException();
            }

            if (!await DomainService.IsProjectLocal(context, ProjectID))
            {
                throw new NonlocalProjectException();
            }

            var task = await TaskCreationPage.Run(context, ProjectID);

            if (task is null)
            {
                return;
            }

            await DomainService.CreateTask(context, ProjectID, task.Name, task.Description, task.Status, task.Priority, task.DueDate);
            await context.SaveChangesAsync();

            string dueDateString = task.DueDate is null ? "" : task.DueDate.Value.ToString("dd-MM-yyyy hh:mm:ss");
            Console.WriteLine($"New task created\nName: {task.Name}, Description: {task.Description}, Status: {task.Status}, Priority: {task.Priority}, Due Date: {dueDateString}");
        }
    }
}
