namespace API.Requests;

public class GetProjectBatchRequest
{
    public List<int> IDs = [];

    public GetProjectBatchRequest(List<int> ids)
    {
        IDs = ids;
    }

    private GetProjectBatchRequest()
    {
        
    }
}
