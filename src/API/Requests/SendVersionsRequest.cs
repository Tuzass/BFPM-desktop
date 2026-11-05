using Infrastructure.Entities;

namespace API.Requests;

public class SendVersionsRequest
{
    public Dictionary<EntityIdPair, int> Versions = [];

    public SendVersionsRequest(Dictionary<EntityIdPair, int> versions)
    {
        Versions = versions;
    }

    private SendVersionsRequest()
    {
        
    }
}
