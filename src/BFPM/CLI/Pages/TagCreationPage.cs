using System.Threading.Tasks;
using Domain.Entities;

namespace BFPM.CLI.Pages;

public class TagCreationPage
{
    public TagCreationPage()
    {
        
    }

    public static async Task<Tag> Run(int projectID)
    {
        string name = "";

        // TODO
        // implement alternate terminal screen where editing happens
        // check for duplicates before returning
        
        return new(null, projectID, name, null);
    }
}
