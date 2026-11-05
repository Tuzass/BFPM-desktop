using System;
using System.Threading.Tasks;
using Infrastructure.Database;
using Services;
using BFPM.CLI.Exceptions;
using BFPM.CLI.Pages;

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

        using (var context = AppDbContextFactory.CreateContext())
        {
            var tag = await DomainService.GetTagByID(context, TagID);

            if (!await DomainService.IsUserAssignedToProject(context, currentUser.ID, tag.ProjectID))
            {
                throw new Services.Exceptions.UnauthorizedAccessException();
            }

            if (!await DomainService.IsProjectLocal(context, tag.ProjectID))
            {
                throw new NonlocalChangeException();
            }

            var localChange = await TagEditPage.Run(context, TagID);

            if (localChange is null)
            {
                return;
            }

            await DomainService.ApplyChange(context, localChange);
            await context.SaveChangesAsync();

            Console.WriteLine($"Tag {TagID} changed\nName: {localChange.Name}");
        }
    }
}
