using Infrastructure.Enums;

namespace Infrastructure.Entities;

public class Notification
{
    public int ID {get; private set;}
    public DomainEntityType? EntityType {get; private set;}
    public int? EntityID {get; private set;}
    public string Content {get; private set;} = null!;
    public bool Deleted {get; private set;}
    public DateTime CreatedAt {get; private set;}

    private Notification()
    {
        
    }

    public Notification(int id, DomainEntityType? entityType, int? entityID, string content)
    {
        ID = id;
        EntityType = entityType;
        EntityID = entityID;
        Content = content;
        Deleted = false;
        CreatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        Deleted = true;
    }
}
