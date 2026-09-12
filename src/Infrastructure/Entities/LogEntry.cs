namespace Infrastructure.Entities;

public class LogEntry
{
    public int ID {get; private set;}
    public int? UserID {get; private set;}
    public int? ProjectID {get; private set;}
    public string Content {get; private set;} = null!;
    public DateTime CreatedAt {get; private set;}

    private LogEntry()
    {
        
    }

    public LogEntry(int id, int? userID, int? projectID, string content)
    {
        ID = id;
        UserID = userID;
        ProjectID = projectID;
        Content = content;
        CreatedAt = DateTime.UtcNow;
    }
}
