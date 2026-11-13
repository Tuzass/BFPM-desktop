using Domain.Enums;

namespace API.Responses;

public class ProjectResponse
{
    public int ID {get; set;}
    public string Name {get; set;} = null!;
    public string Description {get; set;} = null!;
    public int OwnerID {get; set;}
    public string Owner {get; set;} = null!;
    public ProjectStatus Status {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime? CompletedAt {get; set;}
    public int Version {get; set;}
}
