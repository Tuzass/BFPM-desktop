using Avalonia;
using System;

namespace BFPM;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        if (args.Contains("--version"))
        {
            Console.WriteLine("BFPM 1.0.0");
            return;
        }

        if (args.Contains("--nogui"))
        {
            CLI.CLI.Run(args);
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
