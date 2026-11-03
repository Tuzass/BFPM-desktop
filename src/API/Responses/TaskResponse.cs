using Domain.Enums;

namespace API.Responses;

public class TaskResponse
{
    public int ID {get; set;}
    public int ProjectID {get; set;}
    public string Name {get; set;} = null!;
    public string Description {get; set;} = null!;
    public TaskItemStatus Status {get; set;}
    public TaskItemPriority Priority {get; set;}
    public DateTime? DueDate {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime? CompletedAt {get; set;}
    public int Version {get; set;}

    public TaskResponse(int id, int projectID, string name, string description, TaskItemStatus status, TaskItemPriority priority, DateTime? dueDate, DateTime createdAt, DateTime? completedAt, int version)
    {
        ID = id;
        ProjectID = projectID;
        Name = name;
        Description = description;
        Status = status;
        Priority = priority;
        DueDate = dueDate;
        CreatedAt = createdAt;
        CompletedAt = completedAt;
        Version = version;
    }

    private TaskResponse()
    {
        
    }
}
