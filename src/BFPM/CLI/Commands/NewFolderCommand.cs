using System.Threading.Tasks;
using BFPM.CLI.Pages;
using Services;
using Services.Exceptions;

namespace BFPM.CLI.Commands;

public class NewFolderCommand : ICommand
{
    public NewFolderCommand()
    {
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        if (currentUser is null)
        {
            throw new NoCurrentUserException();
        }

        var folder = await FolderCreationPage.Run(currentUser.ID);
        // await DomainService.CreateFolder(folder.Name, folder.Description);
    }
}
