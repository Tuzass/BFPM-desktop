using System.Linq;
using System.Threading.Tasks;
using BFPM.CLI.Exceptions;
using BFPM.CLI.Pages;
using Services;
using Services.Exceptions;

namespace BFPM.CLI.Commands;

public class EditTagCommand : ICommand
{
    public int TagID {get; private set;}

    public EditTagCommand(int tagID)
    {
        TagID = tagID;
    }

    private EditTagCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        var tag = await DomainService.GetTagByID(TagID);

        if (!await DomainService.IsUserAssignedToProject(currentUser.ID, tag.ProjectID))
        {
            throw new UnauthorizedAccessException();
        }

        if (!await DomainService.IsProjectLocal(tag.ProjectID))
        {
            throw new NonlocalChangeException();
        }

        var localChange = await TagEditPage.Run(TagID);

        if (localChange is null)
        {
            return;
        }

        await DomainService.ApplyChange(localChange);
    }
}
