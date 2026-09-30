namespace Domain.Entities;

public class Tag
{
    public int ID {get; private set;}
    public int ProjectID {get; private set;}
    public string Name {get; set;} = null!;

    private Tag()
    {
        
    }

    public Tag(int projectID, string name)
    {
        ProjectID = projectID;
        Name = name;
    }
}
