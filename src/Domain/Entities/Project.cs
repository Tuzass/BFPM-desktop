using Domain.Enums;

namespace Domain.Entities;

public class Project
{
    public int? ID {get; private set;}
    public int OwnerID {get; private set;}
    public int AssignedUserID {get; private set;}
    public string Name {get; set;} = null!;
    public string Description {get; set;} = null!;
    public ProjectStatus Status {get; private set;}
    public bool IsLocal {get; private set;}
    public bool EnableLocalNotifications {get; set;}
    public DateTime CreatedAt {get; private set;}
    public DateTime? CompletedAt {get; private set;}
    public int Version {get; private set;}

    private Project()
    {
        
    }

    public Project(int? id, int ownerID, int assignedUserID, string name, string description, ProjectStatus status, bool isLocal, DateTime? createdAt, DateTime? completedAt, int? version = null)
    {
        ID = id;
        OwnerID = ownerID;
        AssignedUserID = assignedUserID;
        Name = name;
        Description = description;
        Status = status;
        IsLocal = isLocal;
        EnableLocalNotifications = true;
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
