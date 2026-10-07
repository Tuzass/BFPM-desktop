using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Entities;
using Infrastructure.Enums;
using Services.Exceptions;

namespace BFPM.CLI.Pages;

public class FolderEditPage
{
    public FolderEditPage()
    {
        
    }

    public static async Task<LocalChange> Run(Folder folder)
    {
        if (folder.ID is null)
        {
            throw new InexistentFolderException();
        }

        string name = folder.Name;
        string description = folder.Description;

        // TODO
        // implement alternate terminal screen where editing happens
        // check for duplicates before returning

        return new(DomainEntityType.Folder, folder.ID.Value, name, description, null, null, null, false);
    }
}
