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
        var folders = await DomainService.GetCurrentUserFolders();
        var folder = folders.SingleOrDefault(f => f.ID == FolderID);

        if (folder is null)
        {
            throw new UnauthorizedAccessException();
        }

        var localChange = await FolderEditPage.Run(folder);
        // await DomainService.ApplyLocalChange(localChange);
    }
}
