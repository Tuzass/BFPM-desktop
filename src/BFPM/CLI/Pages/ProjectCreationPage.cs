using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;

namespace BFPM.CLI.Pages;

public class ProjectCreationPage
{
    public ProjectCreationPage()
    {
        
    }

    public static async Task<Project> Run(int currentUserID)
    {
        string name = "";
        string description = "";
        ProjectStatus status = ProjectStatus.Active;

        // TODO
        // implement alternate terminal screen where editing happens
        // check for duplicates before returning
        
        return new(null, null, currentUserID, currentUserID, name, description, status, null, null);
    }
}
