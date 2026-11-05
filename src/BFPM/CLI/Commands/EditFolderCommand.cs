using System;
using System.Threading.Tasks;
using Infrastructure.Database;
using Services;
using BFPM.CLI.Pages;

namespace BFPM.CLI.Commands;

public class EditFolderCommand : ICommand
{
    public int FolderID {get; private set;}

    public EditFolderCommand(int folderID)
    {
        FolderID = folderID;
    }

    private EditFolderCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        using (var context = AppDbContextFactory.CreateContext())
        {
            if (!await DomainService.IsFolderOwnedByUser(context, currentUser.ID, FolderID))
            {
                throw new Services.Exceptions.UnauthorizedAccessException();
            }

            var localChange = await FolderEditPage.Run(context, currentUser.ID, FolderID);

            if (localChange is null)
            {
                return;
            }

            await DomainService.ApplyChange(context, localChange);
            await context.SaveChangesAsync();

            Console.WriteLine($"Folder {FolderID} changed\nName: {localChange.Name}, Description: {localChange.Description}");
        }
    }
}
