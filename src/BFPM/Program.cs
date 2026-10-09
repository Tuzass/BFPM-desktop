using Avalonia;
using Services;
using System;

namespace BFPM;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        UserService.CreateLocalUser();

        if (args.Contains("--version"))
        {
            Console.WriteLine("BFPM 1.0.0");
            return;
        }

        if (args.Contains("--no-gui"))
        {
            CLI.Pages.HomePage.Run(args);
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
