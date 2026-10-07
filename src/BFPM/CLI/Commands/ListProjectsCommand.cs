using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Services;

namespace BFPM.CLI.Commands;

public class ListProjectsCommand : ICommand
{
    public ListProjectsCommand()
    {

    }

    public async Task Execute()
    {
        List<Project> projects = await DomainService.GetCurrentUserProjects();

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
