using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Services;
using Services.Exceptions;

namespace BFPM.CLI;

public class HomePage()
{
    private readonly static string StartMessage = @"BFPM 1.0.0 Command-Line Interface
Type 'help' to see the available commands
";

    private readonly static string HelpMessage = @"Available commands: clear, exit, folder, help, list, new, project, quit, settings, users
To get help about a specific command, try 'help <command>'
";

    private static string GetHelpMessage(string command)
    {
        if (command == "clear")
        {
            return "Clears the terminal\n";
        }

        if (command == "exit")
        {
            return "Exits the current page; if in main page, exits the program\n";
        }

        if (command == "folder")
        {
            return @"Usage: folder <folder id>
Enters the page for the folder specified by <folder id>
";
        }

        if (command == "list")
        {
            return @"Usage: list <entity list>
Prints the entity list specified by <option>
Options: folders  => all folders belonging to the current user
         projects => all projects belonging to the current user
";
        }

        if (command == "new")
        {
            return @"Usage: new <entity>
Creates a new entity specified by <entity>
Options: folder  => enters the folder creation page
         project => enters the project creation page
To create new tasks or tags for a certain project, first enter the project page with 'project <project id>'
";
        }

        if (command == "project")
        {
            return @"Usage: project <project id>
Enters the page for the project specified by <project id>
";
        }

        if (command == "quit")
        {
            return "Exits the program in any page\n";
        }

        if (command == "project")
        {
            return "Enters the settings page\n";
        }

        if (command == "users")
        {
            return "Enters the users page\n";
        }

        else
        {
            return "Error: command not found!\n";
        }
    }

    public static async void Run(string[] args)
    {
        var settings = SettingsService.GetSettings();
        Console.WriteLine(StartMessage);

        while (true)
        {
            Console.Write("> ");
            string? inputString = Console.ReadLine();

            if (inputString is null || inputString.Trim() == "")
            {
                Console.WriteLine();
                continue;
            }

            if (inputString.Trim() == "clear")
            {
                Console.Clear();
                continue;
            }

            if (inputString.Trim() == "exit" || inputString.Trim() == "quit")
            {
                break;
            }

            if (inputString.Trim() == "folder")
            {
                Console.WriteLine("No folder ID specified; usage: folder <folder id>\n");
                continue;
            }

            if (inputString.Trim() == "help")
            {
                Console.WriteLine(HelpMessage);
                continue;
            }

            if (inputString.Trim() == "list")
            {
                Console.WriteLine("No entity list specified; usage: list folders, list projects\n");
                continue;
            }

            if (inputString.Trim() == "new")
            {
                Console.WriteLine("No entity specified; usage: new folder, new project\n");
                continue;
            }

            if (inputString.Trim() == "project")
            {
                Console.WriteLine("No project ID specified; usage: project <project id>\n");
                continue;
            }

            if (inputString.Trim() == "settings")
            {
                if (await SettingsPage.Run(settings))
                {
                    break;
                }

                continue;
            }

            if (inputString.Trim() == "users")
            {
                if (await UserPage.Run(settings))
                {
                    break;
                }

                settings = SettingsService.GetSettings();
                continue;
            }

            string[] input = Utils.ParseWords(inputString);

            if (input[0] == "help")
            {
                Console.WriteLine(GetHelpMessage(input[1]));
                continue;
            }

            else if (input[0] == "folder")
            {
                if (int.TryParse(input[1], out int id))
                {
                    try
                    {
                        if (await FolderPage.Run(settings, id))
                        {
                            break;
                        }

                        settings = SettingsService.GetSettings();
                    }

                    catch (NoCurrentUserException)
                    {
                        Console.WriteLine("No user is currently selected\nEnter 'users' to go to the users page and select a user\n");
                    }

                    catch (Services.Exceptions.UnauthorizedAccessException)
                    {
                        Console.WriteLine("The current user is not authorized to see the requested folder\n");
                    }
                }

                else
                {
                    Console.WriteLine("Error: invalid ID!");
                }
            }

            else if (input[0] == "list")
            {
                if (input[1] == "folders")
                {
                    try
                    {
                        SettingsService.UpdateSettings(settings);
                        List<Folder> folders = await DomainService.GetCurrentUserFolders();

                        if (folders.Count == 0)
                        {
                            Console.WriteLine("The current user doesn't own any folders\n");
                        }

                        foreach (var folder in folders)
                        {
                            Console.WriteLine(Utils.FolderToString(folder) + '\n');
                        }
                    }

                    catch (NoCurrentUserException)
                    {
                        Console.WriteLine("No user is currently selected\nEnter 'users' to go to the users page and select a user\n");
                    }
                }

                else if (input[1] == "projects")
                {
                    try
                    {
                        SettingsService.UpdateSettings(settings);
                        var projects = await DomainService.GetCurrentUserProjects();

                        if (projects.Count == 0)
                        {
                            Console.WriteLine("The current user isn't assigned to any projects\n");
                        }

                        foreach (var project in projects)
                        {
                            Console.WriteLine(Utils.ProjectToString(project) + '\n');
                        }
                    }

                    catch (NoCurrentUserException)
                    {
                        Console.WriteLine("No user is currently selected\nEnter 'users' to go to the users page and select a user\n");
                    }
                }

                else if (input[1] == "settings")
                {
                    foreach (var setting in settings)
                    {
                        Console.WriteLine($"{setting.Key} = {setting.Value}");
                    }
                }

                else
                {
                    Console.WriteLine("Error: invalid command!\n");
                }
            }

            else if (input[0] == "new")
            {
                if (input[1] == "folder")
                {
                    // TODO: gathers name and description to create a new folder
                    Console.WriteLine("Creating new folders is currently not possible!\n");
                }

                else if (input[1] == "project")
                {
                    // TODO: gathers name and description to create a new folder
                    Console.WriteLine("Creating new projects is currently not possible!\n");
                }

                else
                {
                    Console.WriteLine("Error: invalid command!\n");
                }
            }

            else if (input[0] == "project")
            {
                if (int.TryParse(input[1], out int id))
                {
                    Console.Write("\x1b[?1049h");
                    if (await ProjectPage.Run(settings, id))
                    {
                        break;
                    }

                    settings = SettingsService.GetSettings();
                }

                else
                {
                    Console.WriteLine("Error: invalid ID!\n");
                }
            }

            else if (input[0] == "clear" || input[0] == "exit" || input[0] == "quit" || input[0] == "settings" || input[0] == "users")
            {
                Console.WriteLine("Error: extra arguments!\n");
            }

            else
            {
                Console.WriteLine("Error: unknown command!\n");
            }
        }

        Console.WriteLine("Exiting...");
    }
}
