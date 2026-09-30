using Infrastructure.Enums;

namespace Infrastructure.Entities;

public class LogEntry
{
    public int ID {get; private set;}
    public DomainEntityType EntityType {get; private set;}
    public int EntityID {get; private set;}
    public string Content {get; private set;} = null!;
    public DateTime CreatedAt {get; private set;}

    private LogEntry()
    {
        
    }

    public LogEntry(DomainEntityType entityType, int entityID, string content)
    {
        EntityType = entityType;
        EntityID = entityID;
        Content = content;
        CreatedAt = DateTime.UtcNow;
    }
}
