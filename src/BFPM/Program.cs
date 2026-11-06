using System;
using System.Threading.Tasks;
using Services;
using Avalonia;

namespace BFPM;

sealed class Program
{
    [STAThread]
    public static async Task Main(string[] args)
    {
        UserService.CreateLocalUser();

        if (args.Contains("--version"))
        {
            Console.WriteLine("BFPM 1.0.0");
            return;
        }

        if (args.Contains("--no-gui"))
        {
            await CLI.Pages.HomePage.Run(args);
        }

        else
        {   
            BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
}
