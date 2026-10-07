using System.Threading.Tasks;
using BFPM.CLI.Pages;

namespace BFPM.CLI.Commands;

public class EditCurrentUserCommand : ICommand
{   public EditCurrentUserCommand()
    {
        
    }

    public async Task Execute()
    {
        await CurrentUserEditPage.Run();
    }
}
