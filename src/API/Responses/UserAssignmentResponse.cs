using Infrastructure.Enums;

namespace API.Responses;

public class UserAssignmentResponse
{
    public int UserID {get; set;}
    public string UserName {get; set;} = null!;
    public ProjectRole Role {get; set;}

    public UserAssignmentResponse(int userID, string userName, ProjectRole role)
    {
        UserID = userID;
        UserName = userName;
        Role = role;
    }

    private UserAssignmentResponse()
    {
        
    }
}
