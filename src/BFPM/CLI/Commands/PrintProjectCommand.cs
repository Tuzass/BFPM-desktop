using System;
using System.Linq;
using System.Threading.Tasks;
using Services;

namespace BFPM.CLI.Commands;

public class PrintProjectCommand : ICommand
{
    public int ProjectID {get; private set;}

    public PrintProjectCommand(int projectID)
    {
        ProjectID = projectID;
    }

    private PrintProjectCommand()
    {
        
    }

    public async Task Execute()
    {
        var projects = await DomainService.GetCurrentUserProjects();
        var project = projects.SingleOrDefault(p => p.ID == ProjectID);

        if (project is null)
        {
            throw new Services.Exceptions.UnauthorizedAccessException();
        }

        Console.WriteLine(Utils.ProjectToString(project));
    }
}
