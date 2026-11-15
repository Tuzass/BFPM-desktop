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
            var projects = await DomainService.GetProjectsAssignedToUser(context, currentUser.ID);

            if (projects.Count == 0)
            {
                Console.WriteLine("The current user isn't assigned to any project");
                return;
            }

            Console.WriteLine("ID | Project Name"); // TODO: make it less ugly
            foreach (var project in projects)
            {
                Console.WriteLine($"{project.ID} | {project.Name}");
            }
        }
    }
}
