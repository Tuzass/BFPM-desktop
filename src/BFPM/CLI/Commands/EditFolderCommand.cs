using System.Linq;
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
            throw new UnauthorizedAccessException();
        }

        var folder = await DomainService.GetFolderByID(FolderID);
        var localChange = await FolderEditPage.Run(folder);
        // await DomainService.ApplyLocalChange(localChange);
    }
}
