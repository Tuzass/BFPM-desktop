namespace API.Responses;

public class TagResponse
{
    public int ID {get; set;}
    public int ProjectID {get; set;}
    public string Name {get; set;} = null!;
    public int Version {get; set;}

    public TagResponse(int id, int projectID, string name, int version)
    {
        ID = id;
        ProjectID = projectID;
        Name = name;
        Version = version;
    }

    private TagResponse()
    {
        
    }
}
