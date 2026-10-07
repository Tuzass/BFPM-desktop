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
        var tag = await DomainService.GetTagByID(TagID);

        if (tag is null)
        {
            throw new UnauthorizedAccessException();
        }

        var projects = await DomainService.GetCurrentUserProjects();
        var project = projects.SingleOrDefault(p => p.ID == tag.ProjectID);

        if (project is null)
        {
            throw new UnauthorizedAccessException();
        }

        if (project.RemoteID is not null)
        {
            throw new NonlocalChangeException();
        }

        // await DomainService.DeleteTag(TagID);
    }
}
