using Domain.Enums;

namespace Domain.Entities;

public class TaskItem
{
    public int? ID {get; private set;}
    public int ProjectID {get; private set;}
    public string Name {get; set;} = null!;
    public string Description {get; set;} = null!;
    public TaskItemStatus Status {get; private set;}
    public TaskItemPriority Priority {get; set;}
    public DateTime? DueDate {get; private set;}
    public DateTime CreatedAt {get; private set;}
    public DateTime? CompletedAt {get; private set;}
    public int Version {get; private set;}

    private TaskItem()
    {
        
    }

    public TaskItem(int? id, int projectID, string name, string description, TaskItemStatus status, TaskItemPriority priority, DateTime? dueDate, DateTime? createdAt, DateTime? completedAt, int? version = null)
    {
        ID = id;
        ProjectID = projectID;
        Name = name;
        Description = description;
        Status = status;
        Priority = priority;
        DueDate = dueDate;
        CreatedAt = (createdAt == null) ? DateTime.UtcNow : (DateTime)createdAt;
        CompletedAt = completedAt;

        if (version is null)
        {
            Version = 1;
        }

        else
        {
            Version = version.Value;
        }
    }

    public void IncreaseVersion()
    {
        Version++;
    }

    public bool ChangeStatus(TaskItemStatus newStatus)
    {
        if (newStatus == TaskItemStatus.Completed && Status != TaskItemStatus.Completed)
        {
            return false;
        }

        if (Status == TaskItemStatus.Completed)
        {
            CompletedAt = null;
        }

        Status = newStatus;
        return false;
    }

    public bool ChangeDueDate(DateTime newDueDate)
    {
        if (newDueDate < DateTime.UtcNow)
        {
            return false;
        }

        DueDate = newDueDate;
        return true;
    }

    public void Complete()
    {
        if (Status == TaskItemStatus.Completed)
        {
            return;
        }

        Status = TaskItemStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }
}
