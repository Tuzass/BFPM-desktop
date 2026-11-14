using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Database;
using Infrastructure.Entities;
using Infrastructure.Enums;
using Services;

namespace BFPM.CLI.Pages;

public class LocalTagPage
{
    public LocalTagPage()
    {
        
    }

    private static void PrintTitle(int firstRow, int tagID)
    {
        Console.SetCursorPosition(0, firstRow);
        Console.Write($"Tag {tagID}");
        Console.SetCursorPosition(0, firstRow + 1);
        Console.Write("Belongs to local project");
    }

    private static void PrintFields(int firstRow, List<string> fieldNames, List<string> fieldContents, int longestFieldLength)
    {
        int minCursorHorizontalPosition = longestFieldLength + 2;

        for (int i = 0; i < fieldNames.Count; i++)
        {
            Console.SetCursorPosition(0, firstRow + i);
            Console.Write((fieldNames[i] + ": ").PadRight(minCursorHorizontalPosition) + fieldContents[i]);
        }
    }

    private static void ClearFields(int firstRow, List<string> fieldNames)
    {
        for (int i = 0; i < fieldNames.Count; i++)
        {
            Console.SetCursorPosition(0, firstRow + i);
            Console.Write(new string(' ', 200));
        }
    }

    private static void PrintMainOptions(int firstRow, List<string> optionNames, int optionIndex)
    {
        for (int i = 0; i < optionNames.Count; i++)
        {
            Console.SetCursorPosition(0, firstRow + i);

            if (i == optionIndex)
            {
                Console.Write("> ");
            }

            else
            {
                Console.Write("  ");
            }

            Console.Write(optionNames[i]);
        }
    }

    private static void ClearMainOptions(int firstRow, List<string> optionNames)
    {
        for (int i = 0; i < optionNames.Count; i++)
        {
            Console.SetCursorPosition(0, firstRow + i);
            Console.Write(new string(' ', 100));
        }
    }


    private static void PrintConfirmationMessage(int row, string message)
    {
        Console.SetCursorPosition(0, row);
        Console.Write(message);
    }

    private static void ClearConfirmationMessage(int row)
    {
        Console.SetCursorPosition(0, row);
        Console.Write(new string(' ', 200));
    }

    private static void PrintConfirmationOptions(int firstRow, string option1, string option2, int optionIndex)
    {
        Console.SetCursorPosition(0, firstRow);
        int indicatorIndex = option1.Length / 2;

        if (optionIndex == 1)
        {
            indicatorIndex = option1.Length + option2.Length / 2 + 3;
        }

        Console.Write(new string(' ', indicatorIndex) + "V" + new string(' ', 50));
        Console.SetCursorPosition(0, firstRow + 1);
        Console.Write($"{option1} | {option2}");
    }

    private static void ClearConfirmationOptions(int firstRow)
    {
        Console.SetCursorPosition(0, firstRow);
        Console.Write(new string(' ', 200));

        Console.SetCursorPosition(0, firstRow + 1);
        Console.Write(new string(' ', 200));
    }
    
    private static void PrintEditingKeyInstructions(int firstRow)
    {
        Console.SetCursorPosition(0, firstRow);
        Console.Write(@"Status options: 1 (Active), 2 (On Hold), 3 (Archived), 4 (Completed)
Enter      | Exit & Save
ESC        | Exit without saving
Up         | Move to the field above
Down/Tab   | Move to the field below
Left/Right | Move cursor within field");
    }

    private static void ClearEditingKeyInstructions(int firstRow)
    {
        Console.SetCursorPosition(0, firstRow);
        Console.Write(new string(' ', 100));

        Console.SetCursorPosition(0, firstRow + 1);
        Console.Write(new string(' ', 100));

        Console.SetCursorPosition(0, firstRow + 2);
        Console.Write(new string(' ', 100));

        Console.SetCursorPosition(0, firstRow + 3);
        Console.Write(new string(' ', 100));

        Console.SetCursorPosition(0, firstRow + 4);
        Console.Write(new string(' ', 100));

        Console.SetCursorPosition(0, firstRow + 5);
        Console.Write(new string(' ', 100));
    }

    private static void PrintErrorMessage(int row, string message)
    {
        Console.SetCursorPosition(0, row);
        Console.Write(message);
    }

    private static void ClearErrorMessage(int row)
    {
        Console.SetCursorPosition(0, row);
        Console.Write(new string(' ', 200));
    }

    public static async Task<List<string>> Run(AppDbContext context, int tagID)
    {
        var currentUser = await UserService.GetCurrentUser();
        var tag = await DomainService.GetTagByID(context, tagID);
        Change change = new(currentUser.ID, DomainEntityType.Tag, tagID, null, null, null, null, null, false);
        List<string> logs = [];

        int titleFirstRow = 0;
        int fieldsFirstRow = 3;
        int mainOptionsFirstRow = 5;
        int confirmationMessageRow = 8;
        int keyInstructionsFirstRow = 8;
        int errorMessageRow = 12;

        List<string> fieldNames = ["Name"];
        List<string> fieldContents = [tag.Name];
        int fieldCount = fieldNames.Count;
        int fieldIndex;

        int longestFieldLength = 0;
        foreach (var fieldName in fieldNames)
        {
            if (fieldName.Length > longestFieldLength)
            {
                longestFieldLength = fieldName.Length;
            }
        }

        int minCursorHorizontalPosition = longestFieldLength + 2;
        List<string> optionNames = ["Edit fields", "Exit"];
        int optionCount = optionNames.Count;
        int optionIndex = 0;
        int cursorIndex;

        Console.Write("\x1b[?1049h");
        Console.Write("\x1b[?7l");
        Console.Clear();
        PrintTitle(titleFirstRow, tagID);

        while (true)
        {
            PrintFields(fieldsFirstRow, fieldNames, fieldContents, longestFieldLength);
            PrintMainOptions(mainOptionsFirstRow, optionNames, optionIndex);

            Console.CursorVisible = false;
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
            {
                ClearMainOptions(mainOptionsFirstRow, optionNames);
                ClearErrorMessage(errorMessageRow);

                if (optionIndex == 0)
                {
                    List<string> oldFieldContents = [fieldContents[0], fieldContents[1], fieldContents[2]];
                    fieldIndex = 0;
                    cursorIndex = minCursorHorizontalPosition + fieldContents[fieldIndex].Length;
                    ClearMainOptions(mainOptionsFirstRow, optionNames);
                    PrintEditingKeyInstructions(keyInstructionsFirstRow);
                    Console.SetCursorPosition(cursorIndex, 3 + fieldIndex);

                    bool shouldExit = false;
                    while (!shouldExit)
                    {
                        Console.CursorVisible = true;
                        var editKey = Console.ReadKey(true);

                        if (editKey.Key == ConsoleKey.Enter)
                        {
                            Console.CursorVisible = false;

                            if (!Utils.ValidateDomainEntityName(fieldContents[0].Trim()))
                            {
                                PrintErrorMessage(errorMessageRow, "Please make sure the tag's name has 3-50 characters and contains only letters, numbers, (, ), -, and spaces!");
                                continue;
                            }

                            if (await DomainService.IsTagNameDuplicate(context, tag.ProjectID, fieldContents[0].Trim(), tag.ID))
                            {
                                PrintErrorMessage(errorMessageRow, $"Project already has a tag named '{fieldContents[0].Trim()}' (case-insensitive)!");
                                continue;
                            }

                            int confirmationIndex = 0;
                            ClearEditingKeyInstructions(keyInstructionsFirstRow);
                            PrintConfirmationMessage(confirmationMessageRow, $"Do you wish to save your changes to tag {tagID}'s attributes?");
                            PrintConfirmationOptions(confirmationMessageRow + 2, "Save changes", "Keep editing", confirmationIndex);

                            while (true)
                            {
                                var confirmationKey = Console.ReadKey(true);

                                if (confirmationKey.Key == ConsoleKey.LeftArrow)
                                {
                                    confirmationIndex = (confirmationIndex - 1 + 2) % 2;
                                    ClearConfirmationOptions(confirmationMessageRow + 2);
                                    PrintConfirmationOptions(confirmationMessageRow + 2, "Save changes", "Keep editing", confirmationIndex);
                                }
                                
                                if (confirmationKey.Key == ConsoleKey.RightArrow)
                                {
                                    confirmationIndex = (confirmationIndex + 1) % 2;
                                    ClearConfirmationOptions(confirmationMessageRow + 2);
                                    PrintConfirmationOptions(confirmationMessageRow + 2, "Save changes", "Keep editing", confirmationIndex);
                                }

                                if (confirmationKey.Key == ConsoleKey.Enter)
                                {
                                    ClearConfirmationOptions(confirmationMessageRow + 2);
                                    ClearConfirmationMessage(confirmationMessageRow);

                                    if (confirmationIndex == 1)
                                    {
                                        PrintEditingKeyInstructions(keyInstructionsFirstRow);
                                        Console.SetCursorPosition(cursorIndex, 3 + fieldIndex);
                                        Console.CursorVisible = true;
                                        break;
                                    }

                                    string newName = fieldContents[0].Trim();

                                    change.Name = newName == tag.Name ? null : newName;
                                    await DomainService.ApplyChange(context, change);
                                    await context.SaveChangesAsync();

                                    logs.Add("Tag's attributes were modified.");

                                    change.Name = null;
                                    change.Description = null;
                                    change.Status = null;

                                    shouldExit = true;
                                    break;
                                }
                            }
                        }

                        if (editKey.Key == ConsoleKey.Escape)
                        {
                            Console.CursorVisible = false;
                            int confirmationIndex = 0;
                            ClearEditingKeyInstructions(keyInstructionsFirstRow);
                            PrintConfirmationMessage(confirmationMessageRow, "Are you sure you want to exit the editing page? Your modifications will be lost.");
                            PrintConfirmationOptions(confirmationMessageRow + 2, "Exit without saving", "Keep editing", confirmationIndex);

                            while (true)
                            {
                                var confirmationKey = Console.ReadKey(true);

                                if (confirmationKey.Key == ConsoleKey.LeftArrow)
                                {
                                    confirmationIndex = (confirmationIndex - 1 + 2) % 2;
                                    ClearConfirmationOptions(confirmationMessageRow + 2);
                                    PrintConfirmationOptions(confirmationMessageRow + 2, "Exit without saving", "Keep editing", confirmationIndex);
                                }
                                
                                if (confirmationKey.Key == ConsoleKey.RightArrow)
                                {
                                    confirmationIndex = (confirmationIndex + 1) % 2;
                                    ClearConfirmationOptions(confirmationMessageRow + 2);
                                    PrintConfirmationOptions(confirmationMessageRow + 2, "Exit without saving", "Keep editing", confirmationIndex);
                                }

                                if (confirmationKey.Key == ConsoleKey.Enter)
                                {
                                    ClearConfirmationOptions(confirmationMessageRow + 2);
                                    ClearConfirmationMessage(confirmationMessageRow);

                                    if (confirmationIndex == 1)
                                    {
                                        PrintEditingKeyInstructions(keyInstructionsFirstRow);
                                        Console.SetCursorPosition(cursorIndex, 3 + fieldIndex);
                                        Console.CursorVisible = true;
                                        break;
                                    }

                                    fieldContents = [oldFieldContents[0], oldFieldContents[1], oldFieldContents[2]];
                                    ClearFields(fieldsFirstRow, fieldNames);
                                    PrintFields(fieldsFirstRow, fieldNames, fieldContents, longestFieldLength);

                                    shouldExit = true;
                                    break;
                                }
                            }
                        }

                        if (editKey.Key == ConsoleKey.UpArrow)
                        {
                            fieldIndex = (fieldIndex - 1 + fieldCount) % fieldCount;
                            cursorIndex = minCursorHorizontalPosition + fieldContents[fieldIndex].Length;
                            Console.SetCursorPosition(cursorIndex, 3 + fieldIndex);
                            continue;
                        }

                        if (editKey.Key == ConsoleKey.DownArrow || editKey.Key == ConsoleKey.Tab)
                        {
                            fieldIndex = (fieldIndex + 1) % fieldCount;
                            cursorIndex = minCursorHorizontalPosition + fieldContents[fieldIndex].Length;
                            Console.SetCursorPosition(cursorIndex, 3 + fieldIndex);
                            continue;
                        }

                        if (editKey.Key == ConsoleKey.LeftArrow)
                        {
                            if (cursorIndex == minCursorHorizontalPosition)
                            {
                                continue;
                            }

                            cursorIndex--;
                            Console.SetCursorPosition(cursorIndex, 3 + fieldIndex);
                            continue;
                        }

                        if (editKey.Key == ConsoleKey.RightArrow)
                        {
                            if (cursorIndex == minCursorHorizontalPosition + fieldContents[fieldIndex].Length)
                            {
                                continue;
                            }

                            cursorIndex++;
                            Console.SetCursorPosition(cursorIndex, 3 + fieldIndex);
                            continue;
                        }

                        if (editKey.Key == ConsoleKey.Backspace)
                        {
                            if (cursorIndex == minCursorHorizontalPosition)
                            {
                                continue;
                            }

                            fieldContents[fieldIndex] = fieldContents[fieldIndex].Remove(cursorIndex - minCursorHorizontalPosition - 1, 1);
                            cursorIndex--;
                            Console.SetCursorPosition(minCursorHorizontalPosition, 3 + fieldIndex);
                            Console.Write(fieldContents[fieldIndex] + " ");
                            Console.SetCursorPosition(cursorIndex, 3 + fieldIndex);
                            continue;
                        }

                        if (!char.IsControl(editKey.KeyChar)
                         && (char.IsLetterOrDigit(editKey.KeyChar)
                          || char.IsPunctuation(editKey.KeyChar)
                          || char.IsWhiteSpace(editKey.KeyChar)))
                        {
                            fieldContents[fieldIndex] = fieldContents[fieldIndex].Insert(cursorIndex - minCursorHorizontalPosition, editKey.KeyChar.ToString());
                            cursorIndex++;
                            Console.SetCursorPosition(minCursorHorizontalPosition, 3 + fieldIndex);
                            Console.Write(fieldContents[fieldIndex]);
                            Console.SetCursorPosition(cursorIndex, 3 + fieldIndex);
                        }
                    }
                }
            }
        
            else if (key.Key == ConsoleKey.UpArrow)
            {
                optionIndex = (optionIndex - 1 + optionCount) % optionCount;
                PrintMainOptions(mainOptionsFirstRow, optionNames, optionIndex);
            }
            
            else if (key.Key == ConsoleKey.DownArrow)
            {
                optionIndex = (optionIndex + 1) % optionCount;
                PrintMainOptions(mainOptionsFirstRow, optionNames, optionIndex);
            }
        }
    }
}
