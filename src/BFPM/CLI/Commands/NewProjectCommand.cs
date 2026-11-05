using System;
using System.Threading.Tasks;
using Infrastructure.Database;
using Services;
using BFPM.CLI.Pages;

namespace BFPM.CLI.Commands;

public class NewProjectCommand : ICommand
{
    public NewProjectCommand()
    {
        
    }

    public async Task Execute()
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            var currentUser = await UserService.GetCurrentUser();
            var project = await ProjectCreationPage.Run(context, currentUser.ID);

            if (project is null)
            {
                return;
            }

            await DomainService.CreateProject(context, currentUser.ID, project.Name, project.Description, project.Status);
            await context.SaveChangesAsync();

            Console.WriteLine($"New project created\nName: {project.Name}, Description: {project.Description}, Status: {project.Status}");
        }
    }
}
