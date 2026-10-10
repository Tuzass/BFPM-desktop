using System;
using System.Threading.Tasks;
using BFPM.CLI.Pages;
using Services;
using BFPM.CLI.Exceptions;
using Microsoft.VisualBasic;

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

        if (!await DomainService.IsUserAssignedToProject(currentUser.ID, ProjectID))
        {
            throw new Services.Exceptions.UnauthorizedAccessException();
        }

        if (!await DomainService.IsProjectLocal(ProjectID))
        {
            throw new NonlocalChangeException();
        }

        var task = await TaskCreationPage.Run(ProjectID);

        if (task is null)
        {
            return;
        }

        string dueDateString = task.DueDate is null ? "" : task.DueDate.Value.ToString("dd-MM-yyyy hh:mm:ss");
        Console.WriteLine($"New task created\nName: {task.Name}, Description: {task.Description}, Status: {task.Status}, Priority: {task.Priority}, Due Date: {dueDateString}");
        await DomainService.CreateTask(ProjectID, task.Name, task.Description, task.Status, task.Priority, task.DueDate);
    }
}
