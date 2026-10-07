using System.Threading.Tasks;
using BFPM.CLI.Pages;
using Services;
using Services.Exceptions;

namespace BFPM.CLI.Commands;

public class NewProjectCommand : ICommand
{
    public NewProjectCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        if (currentUser is null)
        {
            throw new NoCurrentUserException();
        }

        var project = await ProjectCreationPage.Run(currentUser.ID);
        // await DomainService.CreateProject(project.Name, project.Description, project.Status);
    }
}
