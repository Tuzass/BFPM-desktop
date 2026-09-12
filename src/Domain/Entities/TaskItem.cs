using Domain.Enums;

namespace Domain.Entities;

public class TaskItem
{
    public int ID {get; private set;}
    public int ProjectID {get; private set;}
    public string Name {get; private set;} = null!;
    public string Description {get; private set;} = null!;
    public TaskItemStatus Status {get; private set;}
    public TaskItemPriority Priority {get; set;}
    public DateTime? DueDate {get; private set;}
    public DateTime CreatedAt {get; private set;}
    public DateTime? CompletedAt {get; private set;}

    public const int MaxDescriptionLength = 500;

    private TaskItem()
    {
        
    }

    public TaskItem(int id, int projectID, string name, string description, TaskItemStatus status, TaskItemPriority priority, DateTime? dueDate, DateTime? createdAt, DateTime? completedAt)
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
    }

    private static bool IsNameValid(string name)
    {
        if (name.Length >= 3 && name.Length <= 30)
        {
            return true;
        }

        return false;
    }

    public bool Rename(string newName)
    {
        if (!IsNameValid(newName))
        {
            return false;
        }

        Name = newName;
        return true;
    }

    public bool ChangeDescription(string newDescription)
    {
        if (newDescription.Length > MaxDescriptionLength)
        {
            return false;
        }

        Description = newDescription;
        return true;
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
