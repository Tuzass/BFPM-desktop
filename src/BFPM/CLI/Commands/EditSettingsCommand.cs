using System.Threading.Tasks;
using BFPM.CLI.Pages;

namespace BFPM.CLI.Commands;

public class EditSettingsCommand : ICommand
{
    public EditSettingsCommand()
    {
        
    }

    public async Task Execute()
    {
        await SettingsEditPage.Run();
    }
}
