using System;
using System.Threading.Tasks;
using System.Net.Http;
using BFPM.CLI.Exceptions;
using Services;
using Services.Exceptions;
using Infrastructure.Database;

namespace BFPM.CLI.Pages;

public class HomePage()
{
    private readonly static string StartMessage = @"BFPM 1.0.0 Command-Line Interface
Type 'help' to see the available commands";

    public static async Task Run(string[] args)
    {
        HttpClient httpClient = new()
        {
            BaseAddress = new Uri("https://localhost:7035"),
            Timeout = TimeSpan.FromSeconds(5)
        };

        ApiClient apiClient = new(httpClient);
        Console.WriteLine(StartMessage);
        int? currentUserID = null;

        try
        {
            var currentUser = await UserService.GetCurrentUser();

            if (currentUser.ID != 0)
            {
                currentUserID = currentUser.ID;
                
                if (!await apiClient.LoginWithPersistentToken(currentUser.ID))
                {
                    Console.WriteLine("Login failed. Please select another user before performing any operations.");
                }

                else
                {
                    currentUser = await UserService.GetCurrentUser();
                    Console.WriteLine($"Currently logged in as: {currentUser.Username}");

                    using (var context = AppDbContextFactory.CreateContext())
                    {
                        var orphanProjectIDs = await SynchronizationService.StartupSynchronization(context, apiClient, currentUser.ID);

                        foreach (var projectID in orphanProjectIDs)
                        {
                            var project = await DomainService.GetProjectByID(context, projectID);
                            var answer = Utils.GetYesNoConfirmationCli($"Project '{project.Name}' (ID: {projectID}) was a local representation a remote project that has been deleted from the server. Its representation will now be deleted as well. Would you like to create a local copy of it?");

                            if (answer)
                            {
                                int copyID = await DomainService.CopyProject(context, currentUser.ID, projectID);
                                Console.WriteLine($"Project was successfully copied! Copy's ID: {copyID}.");
                            }

                            await DomainService.DeleteProject(context, projectID);
                        }

                        await context.SaveChangesAsync();
                    }
                }
            }
        }

        catch (Exception e)
        {
            switch (e)
            {
                case NoCurrentUserException:
                    Console.WriteLine("No user is currently selected.");
                    break;
                
                case ServerUnavailableException:
                    await apiClient.SetCurrentUserID(currentUserID!.Value);
                    var currentUser = await UserService.GetCurrentUser();
                    Console.WriteLine($"Currently logged in as: {currentUser.Username}");
                    Console.WriteLine("The server is currently unavailable.");
                    break;
                
                default:
                    Console.WriteLine(e);
                    throw;
            }
        }

        while (true)
        {
            Console.Write("\n> ");
            string? inputString = Console.ReadLine();

            if (inputString is null || inputString.Trim() == "")
            {
                continue;
            }

            try
            {
                var tokens = Lexer.Tokenize(inputString.Trim());
                var command = Parser.Parse(tokens, apiClient);

                if (command is null)
                {
                    break;
                }

                await command.Execute();
                continue;
            }

            catch (Exception e)
            {
                switch (e)
                {
                    case Services.Exceptions.UnauthorizedAccessException:
                    case InexistentFolderException:
                    case InexistentProjectException:
                    case InexistentTaskException:
                    case InexistentTagException:
                        Console.WriteLine("The current user is not authorized to perform this operation!");
                        break;
                    
                    case UnknownTokenException:
                        Console.WriteLine("Unknown token; please try 'help' to see the available commands and how to use them correctly!");
                        break;
                    
                    case UnexpectedTokenException:
                        Console.WriteLine("Unexpected token; please try 'help' to see the available commands and how to use them correctly!");
                        break;
                    
                    case InvalidIdException:
                        Console.WriteLine("Invalid ID(s)!");
                        break;
                    
                    case NonlocalProjectException:
                        Console.WriteLine("Can't make changes to nonlocal projects through the CLI! Please launch the usual BFPM application to make such changes!");
                        break;
                    
                    case NoCurrentUserException:
                        Console.WriteLine("No user is currently selected! Please do 'switch' to select a user!");
                        break;

                    default:
                        Console.WriteLine(e.Message);
                        break;
                }
            }
        }

        Console.WriteLine("Exiting...");
    }
}
