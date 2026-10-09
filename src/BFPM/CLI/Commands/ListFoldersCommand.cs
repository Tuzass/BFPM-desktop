using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Services;

namespace BFPM.CLI.Commands;

public class ListFoldersCommand : ICommand
{
    public ListFoldersCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();
        var folders = await DomainService.GetFoldersByUser(currentUser.ID);

        if (folders.Count == 0)
        {
            Console.WriteLine("The current user doesn't own any folders");
            return;
        }

        Console.WriteLine("ID | Folder Name");
        foreach (var folder in folders)
        {
            Console.WriteLine($"{folder.ID} | {folder.Name}");
        }
    }
}
