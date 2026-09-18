namespace Infrastructure.Entities;

public class TaskTagLink
{
    public int TaskID {get; private set;}
    public int TagID {get; private set;}

    private TaskTagLink()
    {
        
    }

    public TaskTagLink(int taskID, int tagID)
    {
        TaskID = taskID;
        TagID = tagID;
    }
}
