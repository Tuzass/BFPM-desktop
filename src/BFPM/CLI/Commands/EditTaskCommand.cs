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
        var currentUser = await UserService.GetCurrentUser();

        var task = await DomainService.GetTaskByID(ID);

        if (!await DomainService.IsUserAssignedToProject(currentUser.ID, task.ProjectID))
        {
            throw new UnauthorizedAccessException();
        }

        if (!await DomainService.IsProjectLocal(task.ProjectID))
        {
            throw new NonlocalChangeException();
        }

        var localChange = await TaskEditPage.Run(task);
        // await DomainService.ApplyLocalChange(localChange);
    }
}
