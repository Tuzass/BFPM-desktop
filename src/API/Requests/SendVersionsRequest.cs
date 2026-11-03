using Infrastructure.Entities;

namespace API.Requests;

public class SendVersionsRequest
{
    public Dictionary<EntityIdPair, int> Versions = [];
}
