using System;
using System.Threading.Tasks;
using Services;

namespace BFPM.CLI.Commands;

public class ListSettingsCommand : ICommand
{
    public ListSettingsCommand()
    {
        
    }

    public async Task Execute()
    {
        var settings = SettingsService.GetSettings();

        foreach (var setting in settings)
        {
            Console.WriteLine($"{setting.Key} = {setting.Value}");
        }
    }
}
