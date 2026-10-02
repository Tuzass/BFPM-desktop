using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Services;
using Services.Exceptions;

namespace BFPM.CLI;

public class UserPage
{
    private readonly static string HelpMessage = @"Available commands: clear, current, edit, exit, help, list, login, logoff, quit, register, remove, switch
To get help about a specific command, try 'help <command>'
";

private static string GetHelpMessage(string command)
    {
        if (command == "clear")
        {
            return "Clears the terminal\n";
        }

        if (command == "current")
        {
            return "Prints information for the current user\n";
        }

        if (command == "edit")
        {
            return "Edits the current user's information\n";
        }

        if (command == "exit")
        {
            return "Returns to the home page\n";
        }

        if (command == "list")
        {
            return "Lists all users registered locally\n";
        }

        if (command == "login")
        {
            return "Prompts the user for a username and a password of an unregistered user (this requires a network connection)\n";
        }

        if (command == "logoff")
        {
            return "Logs the current user off and forces a password to be entered when switching to it the next time\n";
        }

        if (command == "quit")
        {
            return "Exits the program in any page\n";
        }

        if (command == "register")
        {
            return "Prompts the user for a username, email and password to create a new user (this requires a network connection)\n";
        }

        if (command == "remove")
        {
            return @"Usage: remove <user id>
Removes the user specified by <user id> from the local database, so they must log in again
It is not possible to remove the Local User (ID = 0)
";
        }

        if (command == "switch")
        {
            return @"Usage: switch <user id>
Logs the current user off and makes the user specified by <username> as the new current user
To see the usernames of locally registered users, do 'list users'
If the username has been logged off since registering, a password will be asked
If you wish to create a new user, do 'register' (this requires a network connection)
If you wish to log in as an unregistered user, do 'login' (this requires a network connection)
";
        }

        else
        {
            return "Error: command not found!\n";
        }
    }

    private static async Task PrintTitle()
    {
        Console.WriteLine($"Page: users");
        User? currentUser = await UserService.GetCurrentUser();

        if (currentUser is null)
        {
            Console.WriteLine("No user is currently selected\n");
        }

        else
        {
            Console.WriteLine($"Current user: {currentUser.Username}\n");
        }
    }

    public static async Task<bool> Run(string[] args)
    {
        User? currentUser = await UserService.GetCurrentUser();
        Console.Write("\x1b[?1049h");
        Console.Clear();
        await PrintTitle();

        List<string> actions = ["Entered the Users page"];

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
                await PrintTitle();
                continue;
            }

            if (inputString.Trim() == "current")
            {
                Console.WriteLine(Utils.UserToString(currentUser) + "\n");
                continue;
            }

            if (inputString.Trim() == "edit")
            {
                // TODO: implement user edit page
                // if edit was successful, log actions
                Console.WriteLine("Editing a user is currently not possible!\n");
                continue;
            }

            if (inputString.Trim() == "exit")
            {
                Console.WriteLine("Returning to main page\n");
                break;
            }

            if (inputString.Trim() == "help")
            {
                Console.WriteLine(HelpMessage);
                continue;
            }

            if (inputString.Trim() == "list")
            {
                var users = await UserService.GetUsers();
                
                foreach (var user in users)
                {
                    Console.WriteLine(Utils.UserToString(user) + "\n");
                }

                continue;
            }

            if (inputString.Trim() == "login")
            {
                // TODO: implement login after ApiClient is done
                // if login was successful, log into actions
                Console.WriteLine("Logging in is currently not possible!\n");
            }

            if (inputString.Trim() == "logoff")
            {
                // TODO: implement proper logoff after ApiClient is done

                if (currentUser is null)
                {
                    Console.WriteLine("No user is currently selected\n");
                    continue;
                }
                
                var settings = SettingsService.GetSettings();
                settings["CurrentUserID"] = "";
                SettingsService.UpdateSettings(settings);

                Console.WriteLine($"{currentUser.Username} has logged off\n");
                actions.Add($"{currentUser.Username} has logged off");
                currentUser = null;
                continue;
            }

            if (inputString.Trim() == "quit")
            {
                Console.Write("\x1b[?1049l");

                foreach (var action in actions)
                {
                    Console.WriteLine(action);
                }

                Console.WriteLine("Exited the Users page\n");
                return true;
            }

            if (inputString.Trim() == "register")
            {
                // TODO: implement register after ApiClient is done
                // if register was successful, log into actions
                Console.WriteLine("Registering new users is currently not possible!\n");
            }

            if (inputString.Trim() == "remove")
            {
                Console.WriteLine("No user ID specified; usage: remove <user id>\n");
                continue;
            }

            if (inputString.Trim() == "switch")
            {
                Console.WriteLine("No user ID specified; usage: switch <user id>\n");
                continue;
            }

            string[] input = Utils.ParseWords(inputString);

            if (input[0] == "help")
            {
                Console.WriteLine(GetHelpMessage(input[1]));
            }

            else if (input[0] == "switch")
            {
                if (int.TryParse(input[1], out int id))
                {
                    try
                    {
                        await UserService.SwitchCurrentUser(id);
                        // TODO: implement authentication after ApiClient is done
                        // if authentication fails, no user is current
                        currentUser = await UserService.GetCurrentUser();
                    }

                    catch (UserNotRegisteredLocally)
                    {
                        Console.WriteLine("Error: user is not registered locally!\n");
                        continue;
                    }

                    if (currentUser is null)
                    {
                        Console.WriteLine("Switch failed: no user is currently selected\n");
                        continue;
                    }

                    Console.WriteLine($"Current user switched to: {currentUser.Username}\n");
                    actions.Add($"Current user switched to: {currentUser.Username}");
                    continue;
                }

                Console.WriteLine("Error: invalid ID!");
            }

            else if (input[0] == "remove")
            {
                if (int.TryParse(input[1], out int id))
                {
                    try
                    {
                        if (id == 0)
                        {
                            Console.WriteLine("Error: Local User cannot be removed!\n");
                            continue;
                        }

                        await UserService.RemoveRegisteredUser(id);
                        
                        if (currentUser is not null && currentUser.ID == id)
                        {
                            currentUser = await UserService.GetCurrentUser();
                        }
                    }

                    catch (UserNotRegisteredLocally)
                    {
                        Console.WriteLine("Error: user is not registered locally!\n");
                        continue;
                    }

                    if (currentUser is null)
                    {
                        Console.WriteLine("Previous current user was removed: no user is currently selected\n");
                        continue;
                    }

                    actions.Add($"{currentUser.Username} has been removed");
                    continue;
                }

                Console.WriteLine("Error: invalid ID!");
            }

            else if (input[0] == "clear" || input[0] == "current" || input[0] == "edit" || input[0] == "exit" || input[0] == "list" || input[0] == "login" || input[0] == "logoff" || input[0] == "quit" || input[0] == "register")
            {
                Console.WriteLine("Error: extra arguments!\n");
            }

            else
            {
                Console.WriteLine("Error: unknown command!\n");
            }
        }

        Console.Write("\x1b[?1049l");

        foreach (var action in actions)
        {
            Console.WriteLine(action);
        }

        Console.WriteLine("Exited the Users page\n");
        return false;
    }
}
