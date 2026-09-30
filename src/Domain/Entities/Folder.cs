namespace Domain.Entities;

public class Folder
{
    public int ID {get; private set;}
    public int? OwnerID {get; private set;}
    public string Name {get; private set;} = null!;
    public string Description {get; private set;} = null!;

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

    public bool ChangeDescription(string newDescription)
    {
        if (newDescription.Length > MaxDescriptionLength)
        {
            return false;
        }

        Description = newDescription;
        return true;
    }
}
