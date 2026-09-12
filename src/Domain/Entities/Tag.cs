namespace Domain.Entities;

public class Tag
{
    public int ID {get; private set;}
    public int ProjectID {get; private set;}
    public string Name {get; private set;} = null!;

    private Tag()
    {
        
    }

    public Tag(int id, int projectID, string name)
    {
        ID = id;
        ProjectID = projectID;
        Name = name;
    }

    private static bool IsNameValid(string name)
    {
        if (name.Length >= 3 && name.Length <= 30)
        {
            return true;
        }

        return false;
    }

    public bool Rename(string newName)
    {
        if (!IsNameValid(newName))
        {
            return false;
        }

        Name = newName;
        return true;
    }
}
