using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Entities;
using Infrastructure.Enums;
using Services;
using Services.Exceptions;

namespace BFPM.CLI.Pages;

public class TagEditPage
{
    public TagEditPage()
    {
        
    }

    public static async Task<LocalChange> Run(Tag tag)
    {
        if (tag.ID is null)
        {
            throw new InexistentTagException();
        }

        string name = tag.Name;

        // TODO
        // implement alternate terminal screen where editing happens
        // check for duplicates before returning

        return new(DomainEntityType.Tag, tag.ID.Value, name, null, null, null, null, false);
    }
}
