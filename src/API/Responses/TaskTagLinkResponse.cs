namespace API.Responses;

public class TaskTagLinkResponse
{
    public int TaskID {get; set;}
    public int TagID {get; set;}

    public TaskTagLinkResponse(int taskID, int tagID)
    {
        TaskID = taskID;
        TagID = tagID;
    }

    private TaskTagLinkResponse()
    {
        
    }
}
