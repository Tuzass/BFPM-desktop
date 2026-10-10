using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Services;

namespace BFPM.CLI.Pages;

public class TagCreationPage
{
    public TagCreationPage()
    {
        
    }

    private static void PrintTitle()
    {
        Console.SetCursorPosition(0, 0);
        Console.Write("Tag creation page");
    }

    private static void PrintField(string name)
    {
        Console.SetCursorPosition(0, 2);
        Console.Write("Name: " + name);
    }

    private static void PrintMainOptionIndicator(List<string> optionNames, int optionIndex)
    {
        Console.SetCursorPosition(0, 4);

        int totalLength = string.Join(" | ", optionNames).Length;
        string currentIndicator = "";

        for (int i = 0; i < optionIndex; i++)
        {
            currentIndicator += new string(' ', optionNames[i].Length + 3);
        }

        currentIndicator += new string(' ', optionNames[optionIndex].Length / 2) + "V";
        currentIndicator = currentIndicator.PadRight(totalLength);
        Console.Write(currentIndicator);
    }

    private static void PrintMainOptionList(List<string> optionNames)
    {
        Console.SetCursorPosition(0, 5);
        Console.Write(string.Join(" | ", optionNames));
    }

    private static void ClearMainOptions(List<string> optionNames)
    {
        int optionListLength = string.Join(" | ", optionNames).Length;

        Console.SetCursorPosition(0, 4);
        Console.Write(new string(' ', optionListLength));

        Console.SetCursorPosition(0, 5);
        Console.Write(new string(' ', optionListLength));
    }

    private static void PrintConfirmationOptionIndicator(int optionIndex)
    {
        Console.SetCursorPosition(0, 6);

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
        Console.SetCursorPosition(0, 4);
        Console.Write(message);
    }

    private static void PrintConfirmationOptions()
    {
        Console.SetCursorPosition(0, 7);
        Console.Write("Yes | No");
    }

    private static void ClearConfirmationOptions()
    {
        Console.SetCursorPosition(0, 4);
        Console.Write(new string(' ', 200));

        Console.SetCursorPosition(0, 6);
        Console.Write(new string(' ', 8));

        Console.SetCursorPosition(0, 7);
        Console.Write(new string(' ', 8));
    }
    
    private static void PrintEditingKeyInstructions()
    {
        Console.SetCursorPosition(0, 4);
        Console.Write(@"ESC/Enter | Finishes editing
Left/Right | Moves cursor within field");
    }

    private static void ClearEditingKeyInstructions()
    {
        Console.SetCursorPosition(0, 4);
        Console.Write(new string(' ', 28));

        Console.SetCursorPosition(0, 5);
        Console.Write(new string(' ', 38));
    }

    private static void PrintErrorMessage(string message)
    {
        Console.SetCursorPosition(0, 7);
        Console.Write(message);
    }

    private static void ClearErrorMessage()
    {
        Console.SetCursorPosition(0, 7);
        Console.Write(new string(' ', 200));
    }

    public static async Task<Tag?> Run(int projectID)
    {
        var currentUser = await UserService.GetCurrentUser();

        string name = "";
        int minCursorHorizontalPosition = 6;

        Console.Write("\x1b[?1049h");
        Console.Write("\x1b[?7l");
        Console.Clear();
        PrintTitle();
        PrintField(name);

        List<string> optionNames = ["Edit fields", "Create", "Cancel"];
        int optionCount = 3;
        int optionIndex = 0;

        while (true)
        {
            PrintMainOptionIndicator(optionNames, optionIndex);
            PrintMainOptionList(optionNames);
            Console.CursorVisible = false;
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
            {
                ClearMainOptions(optionNames);
                ClearErrorMessage();

                if (optionIndex == 1)
                {
                    if (!Utils.ValidateDomainEntityName(name.Trim()))
                    {
                        PrintErrorMessage("Please make sure the tag's name has 3-50 characters and contains only letters, numbers, (, ), -, and spaces!");
                        continue;
                    }

                    if (await DomainService.IsTagDuplicate(projectID, name.Trim()))
                    {
                        PrintErrorMessage($"Project already has a tag named '{name.Trim()}' (case-insensitive)!");
                        continue;
                    }

                    int confirmationIndex = 0;
                    ClearMainOptions(optionNames);
                    PrintConfirmationMessage("Are you sure you want to create a new tag with these attributes?");
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
                                PrintMainOptionIndicator(optionNames, optionIndex);
                                PrintMainOptionList(optionNames);
                                break;
                            }

                            Console.CursorVisible = true;
                            Console.Write("\x1b[?7h");
                            Console.Write("\x1b[?1049l");
                            return new(null, projectID, name.Trim(), null);
                        }
                    }

                    continue;
                }

                if (optionIndex == 2)
                {
                    int confirmationIndex = 0;
                    ClearMainOptions(optionNames);
                    PrintConfirmationMessage("Are you sure you want to exit the tag creation page? Your modifications will be lost");
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
                                PrintMainOptionIndicator(optionNames, optionIndex);
                                PrintMainOptionList(optionNames);
                                break;
                            }

                            Console.CursorVisible = true;
                            Console.Write("\x1b[?7h");
                            Console.Write("\x1b[?1049l");
                            return null;
                        }
                    }

                    continue;
                }

                int cursorIndex = minCursorHorizontalPosition + name.Length;
                Console.CursorVisible = true;
                ClearMainOptions(optionNames);
                PrintEditingKeyInstructions();
                Console.SetCursorPosition(cursorIndex, 2);

                while (true)
                {
                    var editKey = Console.ReadKey(true);

                    if (editKey.Key == ConsoleKey.Escape || editKey.Key == ConsoleKey.Enter)
                    {
                        ClearEditingKeyInstructions();
                        break;
                    }

                    if (editKey.Key == ConsoleKey.LeftArrow)
                    {
                        if (cursorIndex == minCursorHorizontalPosition)
                        {
                            continue;
                        }

                        cursorIndex--;
                        Console.SetCursorPosition(cursorIndex, 2);
                        continue;
                    }

                    if (editKey.Key == ConsoleKey.RightArrow)
                    {
                        if (cursorIndex == minCursorHorizontalPosition + name.Length)
                        {
                            continue;
                        }

                        cursorIndex++;
                        Console.SetCursorPosition(cursorIndex, 2);
                        continue;
                    }

                    if (editKey.Key == ConsoleKey.Backspace)
                    {
                        if (cursorIndex == minCursorHorizontalPosition)
                        {
                            continue;
                        }

                        name = name.Remove(cursorIndex - minCursorHorizontalPosition - 1, 1);
                        cursorIndex--;
                        Console.SetCursorPosition(minCursorHorizontalPosition, 2);
                        Console.Write(name + " ");
                        Console.SetCursorPosition(cursorIndex, 2);
                        continue;
                    }

                    if (char.IsLetterOrDigit(editKey.KeyChar) || char.IsPunctuation(editKey.KeyChar) || char.IsWhiteSpace(editKey.KeyChar))
                    {
                        name = name.Insert(cursorIndex - minCursorHorizontalPosition, editKey.KeyChar.ToString());
                        cursorIndex++;
                        Console.SetCursorPosition(minCursorHorizontalPosition, 2);
                        Console.Write(name);
                        Console.SetCursorPosition(cursorIndex, 2);
                    }
                }
            }
        
            else if (key.Key == ConsoleKey.LeftArrow)
            {
                optionIndex = (optionIndex - 1 + optionCount) % optionCount;
                PrintMainOptionIndicator(optionNames, optionIndex);
            }
            
            else if (key.Key == ConsoleKey.RightArrow)
            {
                optionIndex = (optionIndex + 1) % optionCount;
                PrintMainOptionIndicator(optionNames, optionIndex);
            }
        }
    }
}
