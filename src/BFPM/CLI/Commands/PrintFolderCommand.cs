using System;
using System.Threading.Tasks;
using Infrastructure.Database;
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

        using (var context = AppDbContextFactory.CreateContext())
        {
            if (!await DomainService.IsFolderOwnedByUser(context, currentUser.ID, FolderID))
            {
                throw new UnauthorizedAccessException();
            }

            var folder = await DomainService.GetFolderByID(context, FolderID);
            var projects = await DomainService.GetProjectsAssignedToFolder(context, FolderID);
            Console.WriteLine(Utils.FolderToString(folder));
            Console.WriteLine($"Projects: {Utils.ProjectListToString(projects)}");
        }
    }
}
