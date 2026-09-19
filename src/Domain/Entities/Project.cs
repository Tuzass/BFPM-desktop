using Domain.Enums;

namespace Domain.Entities;

public class Project
{
    public int ID {get; private set;}
    public int OwnerID {get; private set;}
    public int? AssignedUserID {get; private set;}
    public string Name {get; private set;} = null!;
    public string Description {get; private set;} = null!;
    public ProjectStatus Status {get; private set;}
    public bool IsLocal {get; private set;}
    public bool EnableLocalNotifications {get; set;}
    public DateTime CreatedAt {get; private set;}
    public DateTime? CompletedAt {get; private set;}

    private const int MaxDescriptionLength = 500;

    private Project()
    {
        
    }

    public Project(int id, int ownerID, string name, string description, ProjectStatus status, bool isLocal, DateTime? createdAt, DateTime? completedAt)
    {
        ID = id;
        OwnerID = ownerID;
        Name = name;
        Description = description;
        Status = status;
        IsLocal = isLocal;
        EnableLocalNotifications = true;
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

    public bool ChangeStatus(ProjectStatus newStatus)
    {
        if (newStatus == ProjectStatus.Completed && Status != ProjectStatus.Completed)
        {
            return false;
        }

        if (Status == ProjectStatus.Completed)
        {
            CompletedAt = null;
        }

        Status = newStatus;
        return false;
    }

    public void Complete()
    {
        if (Status == ProjectStatus.Completed)
        {
            return;
        }

        Status = ProjectStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }
}
