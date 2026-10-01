namespace Domain.Entities;

public class Folder
{
    public int? ID {get; private set;}
    public int? OwnerID {get; private set;}
    public string Name {get; set;} = null!;
    public string Description {get; set;} = null!;
    public int Version {get; private set;}

    private Folder()
    {
        
    }

    public Folder(int? id, int? ownerID, string name, string description, int? version = null)
    {
        ID = id;
        OwnerID = ownerID;
        Name = name;
        Description = description;

        if (version is null)
        {
            Version = 1;
        }

        else
        {
            Version = version.Value;
        }
    }

    public void IncreaseVersion()
    {
        Version++;
    }
}
