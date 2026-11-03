using System;
using System.Threading.Tasks;
using BFPM.CLI.Pages;
using Services;
using Services.Exceptions;

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

        if (!await DomainService.IsFolderOwnedByUser(currentUser.ID, FolderID))
        {
            throw new Services.Exceptions.UnauthorizedAccessException();
        }

        var localChange = await FolderEditPage.Run(currentUser.ID, FolderID);

        if (localChange is null)
        {
            return;
        }

        Console.WriteLine($"Folder {FolderID} changed\nName: {localChange.Name}, Description: {localChange.Description}");
        await DomainService.ApplyChange(localChange);
    }
}
