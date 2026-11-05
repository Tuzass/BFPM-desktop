using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace API.Requests;

public class CreateTaskRequest
{
    [Required]
    public int ProjectID {get; set;}

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Name {get; set;} = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description {get; set;} = string.Empty;

    [Required]
    public TaskItemStatus Status {get; set;}

    [Required]
    public TaskItemPriority Priority {get; set;}

    public DateTime? DueDate {get; set;}

    public CreateTaskRequest(int projectID, string name, string description, TaskItemStatus status, TaskItemPriority priority, DateTime? dueDate)
    {
        ProjectID = projectID;
        Name = name;
        Description = description;
        Status = status;
        Priority = priority;
        DueDate = dueDate;
    }

    private CreateTaskRequest()
    {
        
    }
}
