using System.Threading.Tasks;
using Services;

namespace BFPM.CLI.Pages;

public class SettingsEditPage
{
    public SettingsEditPage()
    {
        
    }

    public static async Task Run()
    {
        var settings = SettingsService.GetSettings();

        // TODO
        // implement alternate terminal screen where editing happens
        // check for valid values
        
        SettingsService.UpdateSettings(settings);
    }
}
