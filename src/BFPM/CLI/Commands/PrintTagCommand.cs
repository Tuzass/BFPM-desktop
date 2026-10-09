using System;
using System.Linq;
using System.Threading.Tasks;
using Services;

namespace BFPM.CLI.Commands;

public class PrintTagCommand : ICommand
{
    public int TagID {get; private set;}

    public PrintTagCommand(int tagID)
    {
        TagID = tagID;
    }

    private PrintTagCommand()
    {
        
    }

    public async Task Execute()
    {
        var tag = await DomainService.GetTagByID(TagID);

        var currentUser = await UserService.GetCurrentUser();

        if (!await DomainService.IsUserAssignedToProject(currentUser.ID, tag.ProjectID))
        {
            throw new UnauthorizedAccessException();
        }

        Console.WriteLine(Utils.TagToString(tag));
    }
}
