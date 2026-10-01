using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Database;
using Services;
using Services.Exceptions;

namespace BFPM.CLI;

public class CLI
{
    private static string GetMainErrorMessage(string command)
    {
        if (command == "clear")
        {
            return @"Clears the terminal";
        }

        if (command == "enter")
        {
            return @"Usage: enter <option>
Enters a new page, specified by <option>, where the user can see/modify different parts of the application
Options: folder <folder id>   => enters the folder page to modify the folder or assign projects to it
         project <project id> => enters the project page to modify the project and create new tasks or tags
         settings             => enters the settings page to modify the current settings
         users                => enters the current user's page to change username, email, or password";
        }

        if (command == "exit")
        {
            return @"Exits the current page; if in main page, exits the program";
        }

        if (command == "list")
        {
            return @"Usage: list <option>
Lists the entities specified by <option>
Options: users    => lists all locally registered users
         folders  => lists all folders belonging to the current user
         projects => lists all projects belonging to the current user";
        }

        if (command == "login")
        {
            return @"Prompts the user for a username and a password of a nonregistered user (this requires a network connection)";
        }

        if (command == "logoff")
        {
            return @"Logs the current user off and forces a password to be entered when switching to it the next time";
        }

        if (command == "quit")
        {
            return @"Exits the program in any page";
        }

        if (command == "register")
        {
            return @"Prompts the user for a username, email and password to create a new user (this requires a network connection)";
        }

        if (command == "switch")
        {
            return @"Usage: switch <user id>
Logs the current user off and makes the user specified by <username> as the new current user
To see the usernames of locally registered users, do 'list users'
If the username has been logged off since registering, a password will be asked
If you wish to create a new user, do 'register' (this requires a network connection)
If you wish to log in as a nonregistered user, do 'login' (this requires a network connection)";
        }

        else
        {
            return ErrorMessage + "command not found!";
        }
    }
    
    private readonly static string ErrorMessage = "Error: ";
    private readonly static string ExitMessage = "Exiting...";
    private readonly static string MainLoopMessage = "> ";
    private readonly static string MainHelpMessage = @"Available commands: clear, enter, exit, help, list, login, logoff, register, switch
To get help about a specific command, try 'help <command>'";
private readonly static string UserHelpMessage = @"Available commands: clear, edit, exit, help
To get help about a specific command, try 'help <command>'";
    private readonly static string StartMessage = @"BFPM 1.0.0";

    public static void Run(string[] args)
    {
        MainPage(args);
    }

    public static async void MainPage(string[] args)
    {
        Console.WriteLine(StartMessage);

        while (true)
        {
            Console.Write(MainLoopMessage);
            string? inputString = Console.ReadLine();

            if (inputString is null || inputString.Trim() == "")
            {
                continue;
            }

            if (inputString.Trim() == "clear")
            {
                Console.Clear();
                continue;
            }

            if (inputString.Trim() == "exit")
            {
                break;
            }

            if (inputString.Trim() == "help")
            {
                Console.WriteLine(MainHelpMessage);
                continue;
            }

            if (inputString.Trim() == "login")
            {
                Console.WriteLine("Entering login page...");
                // TODO: implement login page
                continue;
            }

            if (inputString.Trim() == "logoff")
            {
                Console.WriteLine("Logging current user off");
                UserService.Logoff();
                continue;
            }

            if (inputString.Trim() == "quit")
            {
                break;
            }

            if (inputString.Trim() == "register")
            {
                Console.WriteLine("Entering register page...");
                // TODO: implement register page
                continue;
            }

            string[] input = ParseWords(inputString);

            if (input[0] == "help")
            {
                Console.WriteLine(GetMainErrorMessage(input[1]));
                continue;
            }

            else if (input[0] == "enter")
            {
                if (input[1] == "users")
                {
                    if (await UserPage(args))
                    {
                        break;
                    }

                    continue;
                }

                else if (input[1] == "folder")
                {
                    if (int.TryParse(input[2], out int id))
                    {
                        if (await FolderPage(args, id))
                        {
                            break;
                        }

                        continue;
                    }

                    else
                    {
                        Console.WriteLine(ErrorMessage + "invalid ID!");
                    }
                }

                else if (input[1] == "project")
                {
                    if (int.TryParse(input[2], out int id))
                    {
                        if (await ProjectPage(args, id))
                        {
                            break;
                        }

                        continue;
                    }

                    else
                    {
                        Console.WriteLine(ErrorMessage + "invalid ID!");
                    }
                }

                else if (input[1] == "settings")
                {
                    if (await SettingsPage(args))
                    {
                        break;
                    }

                    continue;
                }

                else
                {
                    Console.WriteLine(ErrorMessage + input[1] + " is not a page!");
                    continue;
                }
            }

            else if (input[0] == "list")
            {
                if (input[1] == "users")
                {
                    List<User> users = await UserService.GetUsers();

                    foreach (var user in users)
                    {
                        Console.WriteLine(UserToString(user) + '\n');
                    }
                }

                else if (input[1] == "folders")
                {
                    List<Folder> folders = await DomainService.GetCurrentUserFolders();

                    foreach (var folder in folders)
                    {
                        Console.WriteLine(FolderToString(folder) + '\n');
                    }
                }

                else if (input[1] == "projects")
                {
                    List<Project> projects = await DomainService.GetCurrentUserProjects();

                    foreach (var project in projects)
                    {
                        Console.WriteLine(ProjectToString(project) + '\n');
                    }
                }

                else if (input[1] == "settings")
                {
                    Dictionary<string, string> settings = SettingsService.GetSettings();

                    foreach (var setting in settings)
                    {
                        Console.WriteLine($"{setting.Key} = {setting.Value}");
                    }
                }

                else
                {
                    Console.WriteLine(ErrorMessage + "invalid command!");
                    continue;
                }
            }
        }

        Console.WriteLine(ExitMessage);
    }

    public static async Task<bool> UserPage(string[] args)
    {
        User? currentUser = await UserService.GetCurrentUser();

        if (currentUser is null)
        {
            throw new NoCurrentUserException();
        }

        Console.WriteLine($"Page: user {currentUser.Username}");

        while (true)
        {
            Console.Write(MainLoopMessage);
            string? inputString = Console.ReadLine();

            if (inputString is null || inputString.Trim() == "")
            {
                continue;
            }

            if (inputString.Trim() == "exit")
            {
                break;
            }

            if (inputString.Trim() == "help")
            {
                Console.WriteLine(UserHelpMessage);
                continue;
            }

            if (inputString.Trim() == "quit")
            {
                return true;
            }
        }

        return false;
    }

    public static async Task<bool> FolderPage(string[] args, int folderID)
    {
        User? currentUser = await UserService.GetCurrentUser();

        if (currentUser is null)
        {
            throw new NoCurrentUserException();
        }

        Folder? folder = await DomainService.GetFolderByID(folderID);

        if (folder is null)
        {
            throw new InexistentFolderException();
        }

        Console.WriteLine($"Page: folder {folder.Name}");
        return false;
    }

    public static async Task<bool> ProjectPage(string[] args, int projectID)
    {
        User? currentUser = await UserService.GetCurrentUser();

        if (currentUser is null)
        {
            throw new NoCurrentUserException();
        }

        Project? project = await DomainService.GetProjectByID(projectID);

        if (project is null)
        {
            throw new InexistentProjectException();
        }

        Console.WriteLine($"Page: project {project.Name}");
        return false;
    }

    public static async Task<bool> SettingsPage(string[] args)
    {
        Console.WriteLine("Page: settings");
        return false;
    }

    public static string[] ParseWords(string inputString)
    {
        string[] input = inputString.Split(' ');
        
        for (int i = 0; i < input.Length - 1; i++)
        {
            input[i] = input[i].Trim([' ', '\n', '\t']);
        }

        return input;
    }

    public static string UserToString(User user)
    {
        return $@"ID: {user.ID}
Username: {user.Username}
Email: {user.Email}";
    }

    public static string FolderToString(Folder folder)
    {
        return $@"ID: {folder.ID}
Name: {folder.Name}
Description: {folder.Description}";
    }

    public static string ProjectToString(Project project)
    {
        return $@"ID: {project.ID}
Name: {project.Name}
Description: {project.Description}
Status: {project.Status}
Created at: {project.CreatedAt}
Completed at: {project.CompletedAt}";
    }

    public static string TaskToString(TaskItem task)
    {
        return $@"ID: {task.ID}
Name: {task.Name}
Description: {task.Description}
Status: {task.Status}
Priority: {task.Priority}
DueDate: {task.DueDate}
Created at: {task.CreatedAt}
Completed at: {task.CompletedAt}";
    }

    public static string TagToString(Tag tag)
    {
        return $@"ID: {tag.ID}
Name: {tag.Name}";
    }
}
