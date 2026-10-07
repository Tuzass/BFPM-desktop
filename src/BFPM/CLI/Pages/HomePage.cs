using System;
using BFPM.CLI.Exceptions;
using Services;
using Services.Exceptions;

namespace BFPM.CLI.Pages;

public class HomePage()
{
    private readonly static string StartMessage = @"BFPM 1.0.0 Command-Line Interface
Type 'help' to see the available commands";

    public static async void Run(string[] args)
    {
        var settings = SettingsService.GetSettings();
        Console.WriteLine(StartMessage);

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
                var tokens = Lexer.Tokenize(inputString);
                var command = Parser.Parse(tokens);

                if (command is null)
                {
                    break;
                }

                await command.Execute();
                continue;
            }

            catch (UnknownTokenException)
            {
                Console.WriteLine("Unknown token; please try 'help' to see the available commands and how to use them correctly!");
            }

            catch (UnexpectedTokenException)
            {
                Console.WriteLine("Unexpected token; please try 'help' to see the available commands and how to use them correctly!");
            }

            catch (InvalidIdException)
            {
                Console.WriteLine("Invalid ID(s)!");
            }

            catch (NonlocalChangeException)
            {
                Console.WriteLine("Can't make changes to nonlocal projects through the CLI! Please launch the usual BFPM application to make such changes!");
            }

            catch (Services.Exceptions.UnauthorizedAccessException)
            {
                Console.WriteLine("The current user is not authorized to perform this operation!");
            }

            catch (NoCurrentUserException)
            {
                Console.WriteLine("No user is currently selected! Please do 'switch' to select a user!");
            }
        }

        Console.WriteLine("Exiting...");
    }
}
