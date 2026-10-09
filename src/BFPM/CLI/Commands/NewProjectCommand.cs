using System;
using System.Threading.Tasks;
using BFPM.CLI.Pages;
using Services;

namespace BFPM.CLI.Commands;

public class NewProjectCommand : ICommand
{
    public NewProjectCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();
        var project = await ProjectCreationPage.Run(currentUser.ID);

        if (project is null)
        {
            return;
        }

        Console.WriteLine($"New project created\nName: {project.Name}, Description: {project.Description}, Status: {project.Status}");
        await DomainService.CreateProject(currentUser.ID, project.Name, project.Description, project.Status);
    }
}
