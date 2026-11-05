using System.Threading.Tasks;
using Infrastructure.Database;
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

        using (var context = AppDbContextFactory.CreateContext())
        {
            if (!await DomainService.IsFolderOwnedByUser(context, currentUser.ID, FolderID))
            {
                throw new UnauthorizedAccessException();
            }

            await DomainService.DeleteFolder(context, FolderID);
            await context.SaveChangesAsync();
        }
    }
}
