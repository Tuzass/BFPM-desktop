using System.Linq;
using System.Threading.Tasks;
using BFPM.CLI.Exceptions;
using BFPM.CLI.Pages;
using Services;
using Services.Exceptions;

namespace BFPM.CLI.Commands;

public class EditTagCommand : ICommand
{
    public int ID {get; private set;}

    public EditTagCommand(int id)
    {
        ID = id;
    }

    private EditTagCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        var tag = await DomainService.GetTagByID(ID);

        if (!await DomainService.IsUserAssignedToProject(currentUser.ID, tag.ProjectID))
        {
            throw new UnauthorizedAccessException();
        }

        if (!await DomainService.IsProjectLocal(tag.ProjectID))
        {
            throw new NonlocalChangeException();
        }

        var localChange = await TagEditPage.Run(tag);
        // await DomainService.ApplyLocalChange(localChange);
    }
}
