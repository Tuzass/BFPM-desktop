using System.Linq;
using System.Threading.Tasks;
using Services;
using Services.Exceptions;

namespace BFPM.CLI.Commands;

public class DeleteFolderCommand : ICommand
{
    public int FolderID {get; private set;}

    public DeleteFolderCommand(int folderID)
    {
        FolderID = folderID;
    }

    private DeleteFolderCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        var folders = await DomainService.GetFoldersByUser(currentUser.ID);
        var folder = folders.SingleOrDefault(f => f.ID == FolderID);

        if (folder is null)
        {
            throw new UnauthorizedAccessException();
        }

        // await DomainService.DeleteFolder(FolderID);
    }
}
