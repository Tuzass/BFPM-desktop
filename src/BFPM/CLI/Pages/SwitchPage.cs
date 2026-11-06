using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace BFPM.CLI.Pages;

public static class SwitchPage
{
    private static void PrintTitle()
    {
        Console.SetCursorPosition(0, 0);
        Console.Write("Select one of the users below");
    }

    private static void PrintUserNames(List<string> userNames, int currentIndex)
    {
        for (int i = 0; i < userNames.Count; i++)
        {
            if (i == currentIndex)
            {
                Console.SetCursorPosition(0, 2 + i);
                Console.Write($"> {userNames[i]}");
                continue;
            }

            Console.SetCursorPosition(0, 2 + i);
            Console.Write($"  {userNames[i]}");
        }
    }

    private static void ClearUserNames(List<string> userNames)
    {
        for (int i = 0; i < userNames.Count; i++)
        {
            Console.SetCursorPosition(0, 2 + i);
            Console.Write(new string(' ', userNames[i].Length + 2));
        }
    }

    private static void PrintConfirmationOptionIndicator(int optionIndex)
    {
        Console.SetCursorPosition(0, 4);

        if (optionIndex == 0)
        {
            Console.Write(" V      ");
        }

        else
        {
            Console.Write("       V");
        }
    }

    private static void PrintConfirmationMessage(string message)
    {
        Console.SetCursorPosition(0, 2);
        Console.Write(message);
    }

    private static void PrintConfirmationOptions()
    {
        Console.SetCursorPosition(0, 5);
        Console.Write("Yes | No");
    }

    private static void ClearConfirmationOptions()
    {
        Console.SetCursorPosition(0, 2);
        Console.Write(new string(' ', 200));

        Console.SetCursorPosition(0, 4);
        Console.Write(new string(' ', 8));

        Console.SetCursorPosition(0, 5);
        Console.Write(new string(' ', 8));
    }

    private static void PrintErrorMessage(string message)
    {
        Console.SetCursorPosition(0, 5);
        Console.Write(message);
    }

    private static void ClearErrorMessage()
    {
        Console.SetCursorPosition(0, 5);
        Console.Write(new string(' ', 200));
    }

    public static async Task<int?> Run(List<User> users, string? errorMessage = null)
    {
        Console.Write("\x1b[?1049h");
        Console.Write("\x1b[?7l");
        Console.Clear();
        PrintTitle();

        int userIndex = 0;
        int userCount = users.Count;

        List<string> userNames = [];
        foreach (var user in users)
        {
            userNames.Add(user.Username);
        }

        ClearUserNames(userNames);
        PrintUserNames(userNames, userIndex);

        if (errorMessage is not null)
        {
            PrintErrorMessage(errorMessage);
        }

        while (true)
        {
            Console.CursorVisible = false;
            var key = Console.ReadKey(true);

            ClearUserNames(userNames);
            PrintUserNames(userNames, userIndex);

            if (errorMessage is not null)
            {
                PrintErrorMessage(errorMessage);
            }

            if (key.Key == ConsoleKey.Enter)
            {
                var selectedUser = users[userIndex];
                int confirmationIndex = 0;

                ClearUserNames(userNames);
                ClearErrorMessage();
                PrintConfirmationMessage($"Are you sure you want to log in as {selectedUser.Username}? You may be asked for your password.");
                PrintConfirmationOptionIndicator(confirmationIndex);
                PrintConfirmationOptions();

                while (true)
                {
                    var confirmationKey = Console.ReadKey(true);

                    if (confirmationKey.Key == ConsoleKey.LeftArrow)
                    {
                        confirmationIndex = (confirmationIndex - 1 + 2) % 2;
                        PrintConfirmationOptionIndicator(confirmationIndex);
                    }
                    
                    if (confirmationKey.Key == ConsoleKey.RightArrow)
                    {
                        confirmationIndex = (confirmationIndex + 1) % 2;
                        PrintConfirmationOptionIndicator(confirmationIndex);
                    }

                    if (confirmationKey.Key == ConsoleKey.Enter)
                    {
                        if (confirmationIndex == 1)
                        {
                            ClearConfirmationOptions();
                            PrintUserNames(userNames, userIndex);
                            
                            if (errorMessage is not null)
                            {
                                PrintErrorMessage(errorMessage);
                            }

                            break;
                        }

                        Console.CursorVisible = true;
                        Console.Write("\x1b[?7h");
                        Console.Write("\x1b[?1049l");
                        return selectedUser.ID;
                    }
                }
            }
        
            else if (key.Key == ConsoleKey.UpArrow)
            {
                userIndex = (userIndex - 1 + userCount) % userCount;
                ClearUserNames(userNames);
                PrintUserNames(userNames, userIndex);
            }
            
            else if (key.Key == ConsoleKey.DownArrow)
            {
                userIndex = (userIndex - 1 + userCount) % userCount;
                ClearUserNames(userNames);
                PrintUserNames(userNames, userIndex);
            }

            else if (key.Key == ConsoleKey.Escape)
            {
                ClearUserNames(userNames);
                ClearErrorMessage();
                Console.CursorVisible = true;
                Console.Write("\x1b[?7h");
                Console.Write("\x1b[?1049l");
                return null;
            }
        }
    }
}
