using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using Services;
using Services.Exceptions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BFPM.CLI;

public class FolderPage
{
    private readonly static string HelpMessage = @"Available commands: assign, clear, delete, edit, exit, help, list, quit, unassign
To get help about a specific command, try 'help <command>'";

    private static string GetHelpMessage(string command)
    {
        if (command == "assign")
        {
            return @"Usage: assign <project id>
Assigns the project specified by <project id> to the folder
If the project is already assigned, no action is taken
";
        }

        if (command == "clear")
        {
            return "Clears the terminal\n";
        }

        if (command == "delete")
        {
            return @"Deletes the folder
The projects assigned to the folder won't get deleted
This action also causes the user to exit the folder's page
";
        }

        if (command == "edit")
        {
            return "Enters the folder edit page\n";
        }

        if (command == "exit")
        {
            return "Returns to the home page\n";
        }

        if (command == "list")
        {
            return @"Usage: list <project category>
Lists all projects according to the <project category> criteria
Options: assigned   => all projects currently assigned to the folder
         unassigned => all projects currently unassigned to the folder
";
        }

        if (command == "unassign")
        {
            return @"Usage: unassign <project id>
Unassigns the project specified by <project id> from the folder
If the project isn't assigned, no action is taken
";
        }

        else
        {
            return "Error: command not found!\n";
        }
    }

    private static void PrintTitle(int folderID)
    {
        Console.WriteLine($"Page: folder {folderID}");
    }

    public static async Task<bool> Run(Dictionary<string, string> settings, int folderID)
    {
        SettingsService.UpdateSettings(settings);
        User? currentUser = await UserService.GetCurrentUser();

        if (currentUser is null)
        {
            throw new NoCurrentUserException();
        }

        Folder? folder = await DomainService.GetFolderByID(folderID);

        if (folder is null || folder.OwnerID != currentUser.ID)
        {
            throw new Services.Exceptions.UnauthorizedAccessException();
        }

        Console.Write("\x1b[?1049h");
        Console.Clear();
        PrintTitle(folderID);
        List<string> logs = [$"Entered Folder {folderID}'s page"];

        while (true)
        {
            Console.Write("> ");
            string? inputString = Console.ReadLine();

            if (inputString is null || inputString.Trim() == "")
            {
                Console.WriteLine();
                continue;
            }

            if (inputString.Trim() == "assign")
            {
                Console.WriteLine("No project ID specified; usage: assign <project id>");
                continue;
            }

            if (inputString.Trim() == "clear")
            {
                Console.Clear();
                PrintTitle(folderID);
                continue;
            }

            if (inputString.Trim() == "delete")
            {
                if (currentUser is null)
                {
                    throw new NoCurrentUserException();
                }

                if (folder.OwnerID != currentUser.ID){
                    
                }

                await DomainService.DeleteFolder(folderID);
                logs.Add($"Folder {folderID} was deleted");
                break;
            }

            if (inputString.Trim() == "edit")
            {
                // TODO: implement folder edit page
                // if edit was successful, log actions
                Console.WriteLine("Editing a folder is currently not possible!\n");
                continue;
            }

            if (inputString.Trim() == "exit")
            {
                break;
            }

            if (inputString.Trim() == "help")
            {
                Console.WriteLine(HelpMessage);
                continue;
            }

            if (inputString.Trim() == "list")
            {
                Console.WriteLine("No category specified; usage: list <project category>");

                continue;
            }

            if (inputString.Trim() == "quit")
            {
                Console.Write("\x1b[?1049l");

                foreach (var action in logs)
                {
                    Console.WriteLine(action);
                }

                Console.WriteLine($"Exited Folder {folderID}'s page\n");
                return true;
            }

            if (inputString.Trim() == "unassign")
            {
                Console.WriteLine("No project ID specified; usage: unassign <project id>\n");
                continue;
            }

            string[] input = Utils.ParseWords(inputString);

            if (input[0] == "help")
            {
                Console.WriteLine(GetHelpMessage(input[1]));
            }

            else if (input[0] == "assign")
            {
                if (int.TryParse(input[1], out int id))
                {
                    if (await DomainService.IsCurrentUserAssignedToProject(id))
                    {
                        Console.WriteLine("The current user is not authorized to assign the requested project\n");
                        continue;
                    }

                    var project = await DomainService.GetProjectByID(id);

                    if (project is null)
                    {
                        Console.WriteLine("The project doesn't exist\n");
                        continue;
                    }

                    using (var context = AppDbContextFactory.CreateContext())
                    {
                        bool isAlreadyAssigned = await context.FolderProjectLinks
                            .AnyAsync(l => l.FolderID == folderID && l.ProjectID == id);
                        
                        if (isAlreadyAssigned)
                        {
                            Console.WriteLine($"Project {id} is already assigned to folder\n");
                            continue;
                        }
                    }

                    await DomainService.AssignProjectToFolder(folderID, id);
                    Console.WriteLine($"Project {id} has been assigned\n");
                    logs.Add($"Project {id} has been assigned\n");
                }

                Console.WriteLine("Error: invalid ID!");
            }

            else if (input[0] == "list")
            {
                if (input[1] == "assigned")
                {
                    var projects = await DomainService.GetProjectsByFolder(folderID);

                    if (projects.Count == 0)
                    {
                        Console.WriteLine("The folder is currently empty\n");
                    }

                    foreach (var project in projects)
                    {
                        Utils.ProjectToString(project);
                    }
                }

                else if (input[1] == "unassigned")
                {
                    var projects = await DomainService.GetCurrentUserProjectsNotAssignedToFolder(folderID);

                    if (projects.Count == 0)
                    {
                        Console.WriteLine("The folder contains all of the current user's projects\n");
                    }

                    foreach (var project in projects)
                    {
                        Utils.ProjectToString(project);
                    }
                }

                else
                {
                    Console.WriteLine("Error: invalid category!\n");
                }
            }

            else if (input[0] == "unassign")
            {
                if (int.TryParse(input[1], out int id))
                {
                    if (await DomainService.IsCurrentUserAssignedToProject(id))
                    {
                        Console.WriteLine("The current user is not authorized to unassign the requested project\n");
                        continue;
                    }

                    var project = await DomainService.GetProjectByID(id);

                    if (project is null)
                    {
                        Console.WriteLine("The project doesn't exist\n");
                        continue;
                    }

                    using (var context = AppDbContextFactory.CreateContext())
                    {
                        bool isAssigned = await context.FolderProjectLinks
                            .AnyAsync(l => l.FolderID == folderID && l.ProjectID == id);
                        
                        if (!isAssigned)
                        {
                            Console.WriteLine($"Project {id} is not assigned to folder\n");
                            continue;
                        }
                    }

                    await DomainService.RemoveProjectFromFolder(folderID, id);
                    Console.WriteLine($"Project {id} has been removed from folder\n");
                    logs.Add($"Project {id} has been removed from folder\n");
                }

                Console.WriteLine("Error: invalid ID!");
            }

            else if (input[0] == "clear" || input[0] == "delete" || input[0] == "edit" || input[0] == "exit" || input[0] == "quit")
            {
                Console.WriteLine("Error: extra arguments!\n");
            }

            else
            {
                Console.WriteLine("Error: unknown command!\n");
            }
        }

        Console.Write("\x1b[?1049l");

        foreach (var log in logs)
        {
            Console.WriteLine(log);
        }

        Console.WriteLine($"Exited Folder {folderID}'s page\n");
        return false;
    }
}
