namespace Domain.Entities;

public class Tag
{
    public int ID {get; private set;}
    public int ProjectID {get; private set;}
    public string Name {get; set;} = null!;
    public int Version {get; private set;}

    private Tag()
    {
        
    }

    public Tag(int projectID, string name, int? version = null)
    {
        ProjectID = projectID;
        Name = name;

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
