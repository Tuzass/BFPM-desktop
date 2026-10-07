using System;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Entities;
using Infrastructure.Enums;
using Services;
using Services.Exceptions;

namespace BFPM.CLI.Pages;

public class TaskEditPage
{
    public TaskEditPage()
    {
        
    }

    public static async Task<LocalChange> Run(TaskItem task)
    {
        if (task.ID is null)
        {
            throw new InexistentTaskException();
        }

        string name = task.Name;
        string description = task.Description;
        TaskItemStatus status = task.Status;
        TaskItemPriority priority = task.Priority;
        DateTime? dueDate = task.DueDate;

        // TODO
        // implement alternate terminal screen where editing happens
        // check for duplicates before returning

        var newStatus = status switch
        {
            TaskItemStatus.Todo => DomainEntityStatus.Todo,
            TaskItemStatus.InProgress => DomainEntityStatus.InProgress,
            TaskItemStatus.OnHold => DomainEntityStatus.OnHold,
            TaskItemStatus.Blocked => DomainEntityStatus.Blocked,
            _ => DomainEntityStatus.Completed
        };
        
        return new(DomainEntityType.Project, task.ID.Value, name, description, newStatus, priority, dueDate, dueDate == task.DueDate);
    }
}
