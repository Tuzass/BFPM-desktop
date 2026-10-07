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
        List<Folder> folders = await DomainService.GetCurrentUserFolders();

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
