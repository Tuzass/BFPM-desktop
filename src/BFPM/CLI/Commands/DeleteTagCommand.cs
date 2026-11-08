using System.Threading.Tasks;
using BFPM.CLI.Exceptions;
using Infrastructure.Database;
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

        using (var context = AppDbContextFactory.CreateContext())
        {
            var tag = await DomainService.GetTagByID(context, TagID);

            if (!await DomainService.IsProjectAssignedToUser(context, currentUser.ID, tag.ProjectID))
            {
                throw new UnauthorizedAccessException();
            }

            if (!await DomainService.IsProjectLocal(context, tag.ProjectID))
            {
                throw new NonlocalChangeException();
            }

            await DomainService.DeleteTag(context, TagID);
            await context.SaveChangesAsync();
        }
    }
}
