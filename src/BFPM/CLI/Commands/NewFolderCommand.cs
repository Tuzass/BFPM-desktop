using System;
using System.Threading.Tasks;
using BFPM.CLI.Pages;
using Domain.Entities;
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
        var folder = await FolderCreationPage.Run(currentUser.ID);

        if (folder is null)
        {
            return;
        }

        Console.WriteLine($"New folder created\nName: {folder.Name}, Description: {folder.Description}");
        await DomainService.CreateFolder(currentUser.ID, folder.Name, folder.Description);
    }
}
