using System;
using System.Threading.Tasks;
using Infrastructure.Database;
using Services;

namespace BFPM.CLI.Commands;

public class ListProjectsCommand : ICommand
{
    public ListProjectsCommand()
    {

    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        using (var context = AppDbContextFactory.CreateContext())
        {
            var projects = await DomainService.GetProjectsByUser(context, currentUser.ID);

            if (projects.Count == 0)
            {
                Console.WriteLine("The current user isn't assigned to any project");
                return;
            }

            Console.WriteLine("ID | Project Name");
            foreach (var project in projects)
            {
                Console.WriteLine($"{project.ID} | {project.Name}");
            }
        }
    }
}
