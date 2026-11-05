using System;
using System.Threading.Tasks;
using Infrastructure.Database;
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
        var currentUser = await UserService.GetCurrentUser();

        using (var context = AppDbContextFactory.CreateContext())
        {
            var tag = await DomainService.GetTagByID(context, TagID);

            if (!await DomainService.IsUserAssignedToProject(context, currentUser.ID, tag.ProjectID))
            {
                throw new UnauthorizedAccessException();
            }

            Console.WriteLine(Utils.TagToString(tag));
        }
    }
}
