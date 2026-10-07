using System.Threading.Tasks;
using Domain.Entities;

namespace BFPM.CLI.Pages;

public class FolderCreationPage
{
    public FolderCreationPage()
    {
        
    }

    public static async Task<Folder> Run(int currentUserID)
    {
        string name = "";
        string description = "";

        // TODO
        // implement alternate terminal screen where editing happens
        // check for duplicates before returning
        
        return new(null, currentUserID, name, description, null);
    }
}
