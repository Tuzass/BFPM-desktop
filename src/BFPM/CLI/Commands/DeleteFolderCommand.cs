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

        if (!await DomainService.IsFolderOwnedByUser(currentUser.ID, FolderID))
        {
            throw new UnauthorizedAccessException();
        }

        await DomainService.DeleteFolder(FolderID);
    }
}
