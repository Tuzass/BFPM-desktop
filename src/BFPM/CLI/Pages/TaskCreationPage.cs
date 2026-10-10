using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using Services;

namespace BFPM.CLI.Pages;

public class TaskCreationPage
{
    public TaskCreationPage()
    {
        
    }

    private static void PrintTitle()
    {
        Console.SetCursorPosition(0, 0);
        Console.Write("Task creation page");
    }

    private static void PrintFields(List<string> fieldNames, List<string> fieldContents, int longestFieldLength)
    {
        int minCursorHorizontalPosition = longestFieldLength + 2;

        for (int i = 0; i < fieldNames.Count; i++)
        {
            Console.SetCursorPosition(0, 2 + i);
            Console.Write((fieldNames[i] + ": ").PadRight(minCursorHorizontalPosition) + fieldContents[i]);
        }
    }

    private static void PrintMainOptionIndicator(List<string> optionNames, int optionIndex)
    {
        Console.SetCursorPosition(0, 8);

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
        Console.SetCursorPosition(0, 9);
        Console.Write(string.Join(" | ", optionNames));
    }

    private static void ClearMainOptions(List<string> optionNames)
    {
        int optionListLength = string.Join(" | ", optionNames).Length;

        Console.SetCursorPosition(0, 8);
        Console.Write(new string(' ', optionListLength));

        Console.SetCursorPosition(0, 9);
        Console.Write(new string(' ', optionListLength));
    }

    private static void PrintConfirmationOptionIndicator(int optionIndex)
    {
        Console.SetCursorPosition(0, 10);

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
        Console.SetCursorPosition(0, 8);
        Console.Write(message);
    }

    private static void PrintConfirmationOptions()
    {
        Console.SetCursorPosition(0, 11);
        Console.Write("Yes | No");
    }

    private static void ClearConfirmationOptions()
    {
        Console.SetCursorPosition(0, 8);
        Console.Write(new string(' ', 200));

        Console.SetCursorPosition(0, 10);
        Console.Write(new string(' ', 10));

        Console.SetCursorPosition(0, 11);
        Console.Write(new string(' ', 10));
    }
    
    private static void PrintEditingKeyInstructions()
    {
        Console.SetCursorPosition(0, 8);
        Console.Write(@"Status options: 1 (To Do), 2 (In Progress)
Priority options: 1 (Low), 2 (Medium), 3 (High), 4 (Urgent)
Date format (leave empty for no due date): dd-mm-yyyy hh:mm:ss (24 hour format) // Example: 30-11-2026 12:00:00
ESC/Enter | Finishes editing
Up | Moves to next field
Down/Tab | Moves to previous field
Left/Right | Moves cursor within field");
    }

    private static void ClearEditingKeyInstructions()
    {
        Console.SetCursorPosition(0, 8);
        Console.Write(new string(' ', 42));

        Console.SetCursorPosition(0, 9);
        Console.Write(new string(' ', 59));

        Console.SetCursorPosition(0, 10);
        Console.Write(new string(' ', 111));
        
        Console.SetCursorPosition(0, 11);
        Console.Write(new string(' ', 28));

        Console.SetCursorPosition(0, 12);
        Console.Write(new string(' ', 24));

        Console.SetCursorPosition(0, 13);
        Console.Write(new string(' ', 34));

        Console.SetCursorPosition(0, 14);
        Console.Write(new string(' ', 38));
    }

    private static void PrintErrorMessage(string message)
    {
        Console.SetCursorPosition(0, 11);
        Console.Write(message);
    }

    private static void ClearErrorMessage()
    {
        Console.SetCursorPosition(0, 11);
        Console.Write(new string(' ', 200));
    }

    public static async Task<TaskItem?> Run(int projectID)
    {
        List<string> fieldNames = ["Name", "Description", "Status", "Priority", "Due date"];
        List<string> fieldContents = ["", "", "", "", ""];
        List<string> statusOptions = ["1", "2"];
        List<string> priorityOptions = ["1", "2", "3", "4"];
        DateTime? dueDate = null;
        int fieldCount = 5;
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

        Console.Write("\x1b[?1049h");
        Console.Write("\x1b[?7l");
        Console.Clear();
        PrintTitle();
        PrintFields(fieldNames, fieldContents, longestFieldLength);

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
                    if (!Utils.ValidateDomainEntityName(fieldContents[0].Trim()))
                    {
                        PrintErrorMessage("Please make sure the task's name has 3-50 characters and contains only letters, numbers, (, ), -, and spaces!");
                        continue;
                    }

                    if (await DomainService.IsTaskDuplicate(projectID, fieldContents[0].Trim()))
                    {
                        PrintErrorMessage($"Current user already has a task named '{fieldContents[0].Trim()}' (case-insensitive)!");
                        continue;
                    }

                    if (!Utils.ValidateDomainEntityDescription(fieldContents[1].Trim()))
                    {
                        PrintErrorMessage("Please make sure it is less than 2000 characters long!");
                        continue;
                    }

                    if (!statusOptions.Contains(fieldContents[2].Trim()))
                    {
                        PrintErrorMessage("Please choose a number from 1-2 for the task status!");
                        continue;
                    }

                    if (!priorityOptions.Contains(fieldContents[3].Trim()))
                    {
                        PrintErrorMessage("Please choose a number from 1-4 for the task status!");
                        continue;
                    }

                    string dueDateString = fieldContents[4].Trim();

                    if (dueDateString.Length > 0)
                    {
                        if (DateTime.TryParseExact(dueDateString, "dd-MM-yyyy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                        {
                            dueDate = parsedDate;
                        }

                        else
                        {
                            PrintErrorMessage("Please insert a due date in the exact format! Example: 30-11-2026 12:00:00 (24 hour format)");
                            continue;
                        }
                    }

                    if (dueDate < DateTime.UtcNow)
                    {
                        PrintErrorMessage("Please insert a date that hasn't passed yet!");
                        continue;
                    }

                    int confirmationIndex = 0;
                    ClearMainOptions(optionNames);
                    PrintConfirmationMessage("Are you sure you want to create a new task with these attributes?");
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

                            fieldContents[2] = fieldContents[2].Trim();
                            TaskItemStatus status = fieldContents[2] switch
                            {
                                "1" => TaskItemStatus.Todo,
                                "2" => TaskItemStatus.InProgress,
                                _ => TaskItemStatus.Todo
                            };

                            fieldContents[3] = fieldContents[3].Trim();
                            TaskItemPriority priority = fieldContents[3] switch
                            {
                                "1" => TaskItemPriority.Low,
                                "2" => TaskItemPriority.Medium,
                                "3" => TaskItemPriority.High,
                                "4" => TaskItemPriority.Urgent,
                                _ => TaskItemPriority.Low
                            };

                            Console.CursorVisible = true;
                            Console.Write("\x1b[?7h");
                            Console.Write("\x1b[?1049l");
                            return new(null, projectID, fieldContents[0].Trim(), fieldContents[1].Trim(), status, priority, dueDate, null, null);
                        }
                    }

                    continue;
                }

                if (optionIndex == 2)
                {
                    int confirmationIndex = 0;
                    ClearMainOptions(optionNames);
                    PrintConfirmationMessage("Are you sure you want to exit the task creation page? Your modifications will be lost");
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

                fieldIndex = 0;
                int cursorIndex = minCursorHorizontalPosition + fieldContents[fieldIndex].Length;
                Console.CursorVisible = true;
                ClearMainOptions(optionNames);
                PrintEditingKeyInstructions();
                Console.SetCursorPosition(cursorIndex, 2 + fieldIndex);

                while (true)
                {
                    var editKey = Console.ReadKey(true);

                    if (editKey.Key == ConsoleKey.Escape || editKey.Key == ConsoleKey.Enter)
                    {
                        ClearEditingKeyInstructions();
                        break;
                    }

                    if (editKey.Key == ConsoleKey.UpArrow)
                    {
                        fieldIndex = (fieldIndex - 1 + fieldCount) % fieldCount;
                        cursorIndex = minCursorHorizontalPosition + fieldContents[fieldIndex].Length;
                        Console.SetCursorPosition(cursorIndex, 2 + fieldIndex);
                        continue;
                    }

                    if (editKey.Key == ConsoleKey.DownArrow || editKey.Key == ConsoleKey.Tab)
                    {
                        fieldIndex = (fieldIndex + 1) % fieldCount;
                        cursorIndex = minCursorHorizontalPosition + fieldContents[fieldIndex].Length;
                        Console.SetCursorPosition(cursorIndex, 2 + fieldIndex);
                        continue;
                    }

                    if (editKey.Key == ConsoleKey.LeftArrow)
                    {
                        if (cursorIndex == minCursorHorizontalPosition)
                        {
                            continue;
                        }

                        cursorIndex--;
                        Console.SetCursorPosition(cursorIndex, 2 + fieldIndex);
                        continue;
                    }

                    if (editKey.Key == ConsoleKey.RightArrow)
                    {
                        if (cursorIndex == minCursorHorizontalPosition + fieldContents[fieldIndex].Length)
                        {
                            continue;
                        }

                        cursorIndex++;
                        Console.SetCursorPosition(cursorIndex, 2 + fieldIndex);
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
                        Console.SetCursorPosition(minCursorHorizontalPosition, 2 + fieldIndex);
                        Console.Write(fieldContents[fieldIndex] + " ");
                        Console.SetCursorPosition(cursorIndex, 2 + fieldIndex);
                        continue;
                    }

                    if (char.IsLetterOrDigit(editKey.KeyChar) || char.IsPunctuation(editKey.KeyChar) || char.IsWhiteSpace(editKey.KeyChar))
                    {
                        fieldContents[fieldIndex] = fieldContents[fieldIndex].Insert(cursorIndex - minCursorHorizontalPosition, editKey.KeyChar.ToString());
                        cursorIndex++;
                        Console.SetCursorPosition(minCursorHorizontalPosition, 2 + fieldIndex);
                        Console.Write(fieldContents[fieldIndex]);
                        Console.SetCursorPosition(cursorIndex, 2 + fieldIndex);
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
