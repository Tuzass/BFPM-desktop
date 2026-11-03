using System.ComponentModel.DataAnnotations;
using Infrastructure.Enums;
using Infrastructure.Entities;
using Workflow.Enums;

namespace API.Requests;

public class CreateActionRequest
{
    [Required]
    public int UserID {get; private set;}

    [Required]
    public int ProjectID {get; private set;}

    [Required]
    public ActionType ActionType {get; private set;}

    [Required]
    public DomainEntityType TargetType {get; private set;}

    [Required]
    public int TargetID {get; private set;}

    public DomainEntityType? RelatedEntityType {get; private set;}

    public int? RelatedEntityID {get; private set;}
    public Change? Change {get; private set;} = null!;

    public CreateActionRequest(int userID, ActionType actionType, DomainEntityType targetType, int targetID, DomainEntityType? relatedEntityType, int? relatedEntityID, Change? change)
    {
        UserID = userID;
        ActionType = actionType;
        TargetType = targetType;
        TargetID = targetID;
        RelatedEntityType = relatedEntityType;
        RelatedEntityID = relatedEntityID;
        Change = change;
    }

    private CreateActionRequest()
    {
        
    }
}
