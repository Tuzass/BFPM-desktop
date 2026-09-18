namespace Infrastructure.Entities;

public class FolderProjectLink
{
    public int FolderID {get; private set;}
    public int ProjectID {get; private set;}

    private FolderProjectLink()
    {
        
    }

    public FolderProjectLink(int folderID, int projectID)
    {
        FolderID = folderID;
        ProjectID = projectID;
    }
}
