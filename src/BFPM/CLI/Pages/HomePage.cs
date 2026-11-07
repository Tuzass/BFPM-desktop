using System;
using System.Threading.Tasks;
using System.Net.Http;
using BFPM.CLI.Exceptions;
using Services;
using Services.Exceptions;

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

        var currentUser = await UserService.GetCurrentUser();
        
        if (currentUser.ID != 0)
        {
            await apiClient.LoginWithPersistentToken(currentUser.ID);   
        }

        try
        {
            currentUser = await UserService.GetCurrentUser();
            Console.WriteLine($"Currently logged in as: {currentUser.Username}");
        }

        catch (NoCurrentUserException)
        {
            Console.WriteLine("No user is currently seleceted");
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
                    
                    case NonlocalChangeException:
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
