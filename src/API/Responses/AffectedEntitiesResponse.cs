using Infrastructure.Entities;

namespace API.Responses;

public class AffectedEntitiesResponse
{
    public int UserActionID {get; set;}
    public List<EntityIdPair> AffectedEntities {get; set;} = [];
}
