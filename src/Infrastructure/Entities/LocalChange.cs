using Domain.Enums;
using Infrastructure.Enums;

namespace Infrastructure.Entities;

public class LocalChange
{
    public int ID {get; private set;}
    public DomainEntityType TargetType {get; private set;}
    public int TargetID {get; private set;}
    public string? Name {get; private set;}
    public string? Description {get; private set;}
    public DomainEntityStatus? Status {get; set;}
    public TaskItemPriority? Priority {get; set;}
    public DateTime? DueDate {get; private set;}

    private const int MaxDescriptionLength = 500;

    private LocalChange()
    {
        
    }

    public LocalChange(DomainEntityType targetType, int targetID, string? name, string? description, DomainEntityStatus? status, TaskItemPriority? priority, DateTime? dueDate)
    {
        TargetType = targetType;
        TargetID = targetID;
        Name = name;
        Description = description;
        Status = status;
        Priority = priority;
        DueDate = dueDate;
    }

    private static bool IsNameValid(string name)
    {
        if (name.Length >= 3 && name.Length <= 30)
        {
            return true;
        }

        return false;
    }

    public bool ChangeName(string newName)
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

    public bool ChangeDueDate(DateTime newDueDate)
    {
        if (newDueDate < DateTime.UtcNow)
        {
            return false;
        }

        DueDate = newDueDate;
        return true;
    }
}
