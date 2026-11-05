using System;
using System.Threading.Tasks;
using Infrastructure.Database;
using Services;
using BFPM.CLI.Pages;

namespace BFPM.CLI.Commands;

public class NewFolderCommand : ICommand
{
    public NewFolderCommand()
    {
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        using (var context = AppDbContextFactory.CreateContext())
        {
            var folder = await FolderCreationPage.Run(context, currentUser.ID);

            if (folder is null)
            {
                return;
            }

            await DomainService.CreateFolder(context, currentUser.ID, folder.Name, folder.Description);
            await context.SaveChangesAsync();

            Console.WriteLine($"New folder created\nName: {folder.Name}, Description: {folder.Description}");
        }
    }
}
