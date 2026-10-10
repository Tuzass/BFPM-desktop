using System;
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
        
        if (!await DomainService.IsFolderOwnedByUser(currentUser.ID, FolderID))
        {
            throw new UnauthorizedAccessException();
        }

        var folder = await DomainService.GetFolderByID(FolderID);
        var projects = await DomainService.GetProjectsByFolder(FolderID);
        Console.WriteLine(Utils.FolderToString(folder));
        Console.WriteLine($"Projects: {Utils.ProjectListToString(projects)}");
    }
}
