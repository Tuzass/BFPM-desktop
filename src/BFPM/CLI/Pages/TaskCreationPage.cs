using System;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;

namespace BFPM.CLI.Pages;

public class TaskCreationPage
{
    public TaskCreationPage()
    {
        
    }

    public static async Task<TaskItem> Run(int projectID)
    {
        string name = "";
        string description = "";
        TaskItemStatus status = TaskItemStatus.Todo;
        TaskItemPriority priority = TaskItemPriority.Low;
        DateTime? dueDate = null;

        // TODO
        // implement alternate terminal screen where editing happens
        // check for duplicates before returning
        
        return new(null, projectID, name, description, status, priority, dueDate, null, null);
    }
}
