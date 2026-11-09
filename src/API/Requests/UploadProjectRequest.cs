using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace API.Requests;

public class UploadProjectRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Name {get; set;} = string.Empty;

    [StringLength(2000)]
    public string Description {get; set;} = string.Empty;

    [Required]
    public ProjectStatus Status {get; set;}

    [Required]
    public List<CreateTaskRequest> Tasks {get; set;} = [];

    [Required]
    public List<CreateTagRequest> Tags {get; set;} = [];

    [Required]
    public List<CreateTaskTagLinkRequest> TaskTagLinks {get; set;} = [];

    public UploadProjectRequest(string name, string description, ProjectStatus status, List<CreateTaskRequest> tasks, List<CreateTagRequest> tags, List<CreateTaskTagLinkRequest> taskTagLinks)
    {
        Name = name;
        Description = description;
        Status = status;
        Tasks = tasks;
        Tags = tags;
        TaskTagLinks = taskTagLinks;
    }

    private UploadProjectRequest()
    {
        
    }
}
