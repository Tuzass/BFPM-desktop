using Infrastructure.Enums;

namespace API.Responses;

public class UserAssignmentResponse
{
    public int UserID {get; set;}
    public string UserName {get; set;} = null!;
    public ProjectRole Role {get; set;}
}
