using System;
using System.Threading.Tasks;

namespace BFPM.CLI.Commands;

public class HelpCommand : ICommand
{
    public string? Command {get; private set;}

    public HelpCommand(string? command)
    {
        Command = command;
    }

    private HelpCommand()
    {
        
    }

    public async Task Execute()
    {
        Console.WriteLine(GetHelpMessage(Command));
    }

    private static string GetHelpMessage(string? command)
    {
        if (command is null)
        {
            return @"Available commands: assign, clear, copy, delete, edit, exit, help, list, login, logoff, new, print, register, remove, switch
To get help about a specific command, try 'help <command>'
";
        }

        if (command == "assign")
        {
            return @"Establishes a relationship between two entities
Usage: assign project <project id> folder <folder id>  => assigns the project specified by <project id> to the folder specified by <folder id>
       assign tag <tag id> task <task id>              => assigns the tag specified by <tag id> to the task specified by <task id>; the task and tag must both belong to the same project";
        }

        if (command == "clear")
        {
            return "Clears the terminal";
        }

        if (command == "copy")
        {
            return @"Creates a copy of a project
Usage: copy <project id>";
        }

        if (command == "delete")
        {
            return @"Deletes an entity
Usage: delete folder <folder id>   => deletes the folder specified by <folder id>; projects assigned to this folder will not be affected
       delete project <project id> => deletes the project specified by <project id>; all tasks and tags belonging to this project will also be deleted
       delete task <task id>       => deletes the task specified by <task id>; tags assigned to this task will not be affected
       delete tag <tag id>         => deletes the tag specified by <tag id>; tasks with these tags will have them removed
       delete user <user id>       => deletes the user from the local database; this action does not delete the user from the server";
        }

        if (command == "edit")
        {
            return @"Edits an entity
Usage: edit folder <folder id>  => enters the edit page for the folder specified by <folder id>
       edit project <project id> => enters the edit page for the project specified by <project id>
       edit task <task id>    => enters the edit page for the task specified by <task id>
       edit tag <tag id>     => enters the edit page for the tag specified by <tag id>";
        }

        if (command == "exit")
        {
            return "Exits the program";
        }

        if (command == "list")
        {
            return @"Prints a list of the information requested
Usage: list folders  => all folders belonging to the current user
       list projects => all projects belonging to the current user
       list settings => current settings";
        }

        if (command == "login")
        {
            return "Enters the 'user login' page";
        }

        if (command == "logoff")
        {
            return @"Logs the current user off and forces a password to be requested in the next switch
Automatically redirects to the 'switch user' page after";
        }

        if (command == "new")
        {
            return @"Creates a new entity
Usage: new folder            => enters the folder creation page
       new project           => enters the project creation page
       new task <project id> => enters the task creation page for project specified by <project id>
       new tag <project id>  => enters the tag creation page for project specified by <project id>";
        }

        if (command == "register")
        {
            return "Enters the 'user register' page";
        }

        if (command == "remove")
        {
            return @"Removes previously established relationships between entities
Usage: remove project <project id> folder <folder id>  => removes the project specified by <project id> from the folder specified by <folder id>
       remove tag <tag id> task <task id>  => removes the tag specified by <tag id> from the task specified by <task id>";
        }

        if (command == "switch")
        {
            return "Enters the 'user switch' page to change the current user";
        }

        else
        {
            return "Error: command not found!";
        }
    }
}
