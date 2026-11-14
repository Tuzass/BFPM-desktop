using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Database;
using Services;
using BFPM.CLI.Pages;

namespace BFPM.CLI.Commands;

public class EnterFolderCommand : ICommand
{
    public int FolderID {get; private set;}

    public EnterFolderCommand(int folderID)
    {
        FolderID = folderID;
    }

    private EnterFolderCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        using (var context = AppDbContextFactory.CreateContext())
        {
            if (!await DomainService.IsFolderOwnedByUser(context, currentUser.ID, FolderID))
            {
                throw new Services.Exceptions.UnauthorizedAccessException();
            }

            List<string> logs = await FolderPage.Run(context, FolderID);

            foreach (var log in logs)
            {
                Console.WriteLine(log);
            }
        }
    }
}
