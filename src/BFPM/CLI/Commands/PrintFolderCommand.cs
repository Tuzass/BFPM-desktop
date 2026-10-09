using System;
using System.Linq;
using System.Threading.Tasks;
using Services;

namespace BFPM.CLI.Commands;

public class PrintFolderCommand : ICommand
{
    public int FolderID {get; private set;}

    public PrintFolderCommand(int folderID)
    {
        FolderID = folderID;
    }

    private PrintFolderCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();
        
        var folders = await DomainService.GetFoldersByUser(currentUser.ID);
        var folder = folders.SingleOrDefault(f => f.ID == FolderID);

        if (folder is null)
        {
            throw new Services.Exceptions.UnauthorizedAccessException();
        }

        Console.WriteLine(Utils.FolderToString(folder));
    }
}
