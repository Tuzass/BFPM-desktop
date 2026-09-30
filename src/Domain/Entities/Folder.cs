namespace Domain.Entities;

public class Folder
{
    public int ID {get; private set;}
    public int? OwnerID {get; private set;}
    public string Name {get; set;} = null!;
    public string Description {get; set;} = null!;

    private const int MaxDescriptionLength = 500;

    private Folder()
    {
        
    }

    public Folder(int ownerID, string name, string description)
    {
        OwnerID = ownerID;
        Name = name;
        Description = description;
    }
}
