using Infrastructure.Entities;

namespace API.Responses;

public class AffectedEntitiesResponse
{
    public List<EntityIdPair> AffectedEntities {get; set;} = [];
}
