namespace Domain.Entities;

public class Tag
{
    public int? ID {get; private set;}
    public int? RemoteID {get; set;}
    public int ProjectID {get; private set;}
    public string Name {get; set;} = null!;
    public int Version {get; private set;}

    private Tag()
    {
        
    }

    public Tag(int? remoteID, int projectID, string name, int? version = null)
    {
        RemoteID = remoteID;
        ProjectID = projectID;
        Name = name;
        Version = version is null ? 1 : version.Value;
    }

    public void IncreaseVersion()
    {
        Version++;
    }
}
