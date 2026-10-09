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

        if (tag is null)
        {
            throw new Services.Exceptions.UnauthorizedAccessException();
        }

        var currentUser = await UserService.GetCurrentUser();

        var projects = await DomainService.GetProjectsByUser(currentUser.ID);
        var project = projects.SingleOrDefault(p => p.ID == tag.ProjectID);

        if (project is null)
        {
            throw new Services.Exceptions.UnauthorizedAccessException();
        }

        Console.WriteLine(Utils.TagToString(tag));
    }
}
