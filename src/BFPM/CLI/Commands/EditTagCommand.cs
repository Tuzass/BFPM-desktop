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
        var tag = await DomainService.GetTagByID(ID);

        if (tag is null)
        {
            throw new UnauthorizedAccessException();
        }

        var currentUser = await UserService.GetCurrentUser();

        var projects = await DomainService.GetProjectsByUser(currentUser.ID);
        var project = projects.SingleOrDefault(p => p.ID == tag.ProjectID);

        if (project is null)
        {
            throw new UnauthorizedAccessException();
        }

        if (project.RemoteID is not null)
        {
            throw new NonlocalChangeException();
        }

        var localChange = await TagEditPage.Run(tag);
        // await DomainService.ApplyLocalChange(localChange);
    }
}
