using System.Linq;
using System.Threading.Tasks;
using BFPM.CLI.Exceptions;
using Services;
using Services.Exceptions;

namespace BFPM.CLI.Commands;

public class DeleteTagCommand : ICommand
{
    public int TagID {get; private set;}

    public DeleteTagCommand(int tagID)
    {
        TagID = tagID;
    }

    private DeleteTagCommand()
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

        // await DomainService.DeleteTag(TagID);
    }
}
