using Domain.Enums;

namespace Domain.Entities;

public class Project
{
    public int ID {get; private set;}
    public int OwnerID {get; private set;}
    public int? AssignedUserID {get; private set;}
    public string Name {get; set;} = null!;
    public string Description {get; set;} = null!;
    public ProjectStatus Status {get; private set;}
    public bool IsLocal {get; private set;}
    public bool EnableLocalNotifications {get; set;}
    public DateTime CreatedAt {get; private set;}
    public DateTime? CompletedAt {get; private set;}

    private const int MaxDescriptionLength = 500;

    private Project()
    {
        
    }

    public Project(int ownerID, string name, string description, ProjectStatus status, bool isLocal, DateTime? createdAt, DateTime? completedAt)
    {

        OwnerID = ownerID;
        Name = name;
        Description = description;
        Status = status;
        IsLocal = isLocal;
        EnableLocalNotifications = true;
        CreatedAt = (createdAt == null) ? DateTime.UtcNow : (DateTime)createdAt;
        CompletedAt = completedAt;
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
