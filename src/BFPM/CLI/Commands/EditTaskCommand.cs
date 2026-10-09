using System.Linq;
using System.Threading.Tasks;
using BFPM.CLI.Exceptions;
using BFPM.CLI.Pages;
using Services;
using Services.Exceptions;

namespace BFPM.CLI.Commands;

public class EditTaskCommand : ICommand
{
    public int ID {get; private set;}

    public EditTaskCommand(int id)
    {
        ID = id;
    }

    private EditTaskCommand()
    {
        
    }

    public async Task Execute()
    {
        var task = await DomainService.GetTaskByID(ID);

        if (task is null)
        {
            throw new UnauthorizedAccessException();
        }

        var currentUser = await UserService.GetCurrentUser();

        var projects = await DomainService.GetProjectsByUser(currentUser.ID);
        var project = projects.SingleOrDefault(p => p.ID == task.ProjectID);

        if (project is null)
        {
            throw new UnauthorizedAccessException();
        }

        if (project.RemoteID is not null)
        {
            throw new NonlocalChangeException();
        }

        var localChange = await TaskEditPage.Run(task);
        // await DomainService.ApplyLocalChange(localChange);
    }
}
