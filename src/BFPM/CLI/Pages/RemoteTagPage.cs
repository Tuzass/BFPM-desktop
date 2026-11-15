using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Database;
using Infrastructure.Entities;
using Infrastructure.Enums;
using Workflow.Enums;
using Services;

namespace BFPM.CLI.Pages;

public class RemoteTagPage
{
    public RemoteTagPage()
    {
        
    }

    private static void PrintTitles(int firstRow, int tagID, int remoteID)
    {
        Console.SetCursorPosition(0, firstRow);
        Console.Write($"Tag {tagID}");
        Console.SetCursorPosition(0, firstRow + 1);
        Console.Write($"Belongs to remote project");

        Console.SetCursorPosition(0, firstRow + 7);
        Console.Write("=====================");
        Console.SetCursorPosition(0, firstRow + 8);
        Console.Write("Your proposed changes");
    }

    private static void PrintFields(int firstRow, List<string> fieldNames, List<string> originalFieldContents, List<string> changeableFieldContents, int longestFieldLength)
    {
        int minCursorHorizontalPosition = longestFieldLength + 2;

        for (int i = 0; i < fieldNames.Count; i++)
        {
            Console.SetCursorPosition(0, firstRow + i);
            Console.Write((fieldNames[i] + ": ").PadRight(minCursorHorizontalPosition) + originalFieldContents[i]);
        }

        for (int i = 0; i < fieldNames.Count; i++)
        {
            Console.SetCursorPosition(0, firstRow + i + 7);
            Console.Write((fieldNames[i] + ": ").PadRight(minCursorHorizontalPosition) + changeableFieldContents[i]);
        }
    }

    private static void ClearFields(int firstRow, List<string> fieldNames)
    {
        for (int i = 0; i < fieldNames.Count; i++)
        {
            Console.SetCursorPosition(0, firstRow + i);
            Console.Write(new string(' ', 200));
        }

        for (int i = 0; i < fieldNames.Count; i++)
        {
            Console.SetCursorPosition(0, firstRow + i + 7);
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

    public static async Task<List<string>> Run(AppDbContext context, ApiClient apiClient, int tagID)
    {
        var currentUser = await UserService.GetCurrentUser();
        var tag = await DomainService.GetTagByID(context, tagID);
        var project = await DomainService.GetProjectByID(context, tag.ProjectID);

        if (tag.RemoteID is null)
        {
            throw new InvalidOperationException("Tried to run remote tag page on local tag.");
        }

        Change change = new(currentUser.ID, DomainEntityType.Tag, tagID, null, null, null, null, null, false);
        List<string> logs = [];

        int titleFirstRow = 0;
        int fieldsFirstRow = 3;
        int mainOptionsFirstRow = 5;
        int confirmationMessageRow = 8;
        int keyInstructionsFirstRow = 8;
        int errorMessageRow = 12;

        List<string> fieldNames = ["Name"];
        List<string> originalFieldContents = [tag.Name];
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
        List<string> optionNames = ["Edit fields", "Commit changes", "Exit"];
        int optionCount = optionNames.Count;
        int optionIndex = 0;
        int cursorIndex;

        List<string> fieldContents = [tag.Name];
        var persistedChange = await DomainService.GetChange(context, DomainEntityType.Tag, tagID);

        if (persistedChange is not null)
        {
            if (persistedChange.Name is not null)
            {
                fieldContents[0] = persistedChange.Name;
            }
        }

        Console.Write("\x1b[?1049h");
        Console.Write("\x1b[?7l");
        Console.Clear();
        PrintTitles(titleFirstRow, tagID, tag.RemoteID.Value);

        while (true)
        {
            PrintFields(fieldsFirstRow, fieldNames, originalFieldContents, fieldContents, longestFieldLength);
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
                    Console.SetCursorPosition(cursorIndex, 10 + fieldIndex);

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
                                Console.SetCursorPosition(cursorIndex, 10 + fieldIndex);
                                continue;
                            }

                            if (await DomainService.IsTagNameDuplicate(context, currentUser.ID, fieldContents[0].Trim(), tagID))
                            {
                                PrintErrorMessage(errorMessageRow, $"Project already has a tag named '{fieldContents[0].Trim()}' (case-insensitive)!");
                                Console.SetCursorPosition(cursorIndex, 10 + fieldIndex);
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
                                        Console.SetCursorPosition(cursorIndex, 10 + fieldIndex);
                                        Console.CursorVisible = true;
                                        break;
                                    }

                                    string newName = fieldContents[0].Trim();

                                    change.Name = newName == tag.Name ? null : newName;
                                    await DomainService.StoreOrReplaceChange(context, change);
                                    await context.SaveChangesAsync();

                                    PrintErrorMessage(errorMessageRow, "Changes were saved. Select 'Commit changes' to submit them to the remote tag.");
                                    logs.Add("Changes were saved locally.");

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
                                        Console.SetCursorPosition(cursorIndex, 10 + fieldIndex);
                                        Console.CursorVisible = true;
                                        break;
                                    }

                                    fieldContents = [oldFieldContents[0], oldFieldContents[1], oldFieldContents[2]];
                                    ClearFields(fieldsFirstRow, fieldNames);
                                    PrintFields(fieldsFirstRow, fieldNames, originalFieldContents, fieldContents, longestFieldLength);

                                    shouldExit = true;
                                    break;
                                }
                            }
                        }

                        if (editKey.Key == ConsoleKey.UpArrow)
                        {
                            fieldIndex = (fieldIndex - 1 + fieldCount) % fieldCount;
                            cursorIndex = minCursorHorizontalPosition + fieldContents[fieldIndex].Length;
                            Console.SetCursorPosition(cursorIndex, 10 + fieldIndex);
                            continue;
                        }

                        if (editKey.Key == ConsoleKey.DownArrow || editKey.Key == ConsoleKey.Tab)
                        {
                            fieldIndex = (fieldIndex + 1) % fieldCount;
                            cursorIndex = minCursorHorizontalPosition + fieldContents[fieldIndex].Length;
                            Console.SetCursorPosition(cursorIndex, 10 + fieldIndex);
                            continue;
                        }

                        if (editKey.Key == ConsoleKey.LeftArrow)
                        {
                            if (cursorIndex == minCursorHorizontalPosition)
                            {
                                continue;
                            }

                            cursorIndex--;
                            Console.SetCursorPosition(cursorIndex, 10 + fieldIndex);
                            continue;
                        }

                        if (editKey.Key == ConsoleKey.RightArrow)
                        {
                            if (cursorIndex == minCursorHorizontalPosition + fieldContents[fieldIndex].Length)
                            {
                                continue;
                            }

                            cursorIndex++;
                            Console.SetCursorPosition(cursorIndex, 10 + fieldIndex);
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
                            Console.SetCursorPosition(minCursorHorizontalPosition, 10 + fieldIndex);
                            Console.Write(fieldContents[fieldIndex] + " ");
                            Console.SetCursorPosition(cursorIndex, 10 + fieldIndex);
                            continue;
                        }

                        if (!char.IsControl(editKey.KeyChar)
                         && (char.IsLetterOrDigit(editKey.KeyChar)
                          || char.IsPunctuation(editKey.KeyChar)
                          || char.IsWhiteSpace(editKey.KeyChar)))
                        {
                            fieldContents[fieldIndex] = fieldContents[fieldIndex].Insert(cursorIndex - minCursorHorizontalPosition, editKey.KeyChar.ToString());
                            cursorIndex++;
                            Console.SetCursorPosition(minCursorHorizontalPosition, 10 + fieldIndex);
                            Console.Write(fieldContents[fieldIndex]);
                            Console.SetCursorPosition(cursorIndex, 10 + fieldIndex);
                        }
                    }
                }

                if (optionIndex == 1) // commit changes
                {
                    ActionType actionType = ActionType.ChangeAttribute;
                    DomainEntityType? relatedEntityType = null;
                    int? relatedEntityID = null;

                    if (tag.RemoteID is null)
                    {
                        actionType = ActionType.CreateEntity;
                        relatedEntityType = DomainEntityType.Project;
                        relatedEntityID = project.RemoteID;
                    }

                    var result = await SynchronizationService.CommitChanges(context, apiClient,
                                                                            currentUser.ID,
                                                                            project.RemoteID!.Value,
                                                                            actionType,
                                                                            DomainEntityType.Tag,
                                                                            tagID,
                                                                            relatedEntityType,
                                                                            relatedEntityID,
                                                                            change);

                    if (result)
                    {
                        PrintErrorMessage(errorMessageRow, "Changes were successfully commited to remote tag! Applying changes to local tag...");

                        await DomainService.ApplyChange(context, change);
                        await DomainService.DeleteStoredChange(context, currentUser.ID, DomainEntityType.Tag, tagID);
                        await context.SaveChangesAsync();

                        ClearErrorMessage(errorMessageRow);
                        PrintErrorMessage(errorMessageRow, "Changes were successfully commited to remote tag! Changes were also applied successfully to the associated local tag! Other entities may have been affected, so make sure to perform synchronization in the corresponding project's page.");
                        logs.Add("Changes were successfully commited to remote tag! Changes were also applied successfully to the associated local tag! Other entities may have been affected, so make sure to perform synchronization in the corresponding project's page.");
                    }

                    else
                    {
                        PrintErrorMessage(errorMessageRow, "Changes were rejected by the server! Please perform synchronization on the corresponding project before submitting your changes again!");
                        logs.Add("Changes were rejected by the server! Please perform synchronization on the corresponding project before submitting your changes again!");
                    }
                }

                if (optionIndex == optionCount - 1) // exit
                {
                    int confirmationIndex = 0;
                    ClearMainOptions(mainOptionsFirstRow, optionNames);
                    PrintConfirmationMessage(confirmationMessageRow, "Are you sure you want to exit the project's page?");
                    PrintConfirmationOptions(confirmationMessageRow + 2, "Yes", "No", confirmationIndex);

                    while (true)
                    {
                        var confirmationKey = Console.ReadKey(true);

                        if (confirmationKey.Key == ConsoleKey.LeftArrow)
                        {
                            confirmationIndex = (confirmationIndex - 1 + 2) % 2;
                            ClearConfirmationOptions(confirmationMessageRow + 2);
                            PrintConfirmationOptions(confirmationMessageRow + 2, "Yes", "No", confirmationIndex);
                        }
                        
                        if (confirmationKey.Key == ConsoleKey.RightArrow)
                        {
                            confirmationIndex = (confirmationIndex + 1) % 2;
                            ClearConfirmationOptions(confirmationMessageRow + 2);
                            PrintConfirmationOptions(confirmationMessageRow + 2, "Yes", "No", confirmationIndex);
                        }

                        if (confirmationKey.Key == ConsoleKey.Enter)
                        {
                            if (confirmationIndex == 1)
                            {
                                ClearConfirmationMessage(confirmationMessageRow);
                                ClearConfirmationOptions(confirmationMessageRow + 2);
                                PrintMainOptions(mainOptionsFirstRow, optionNames, optionIndex);
                                break;
                            }

                            Console.CursorVisible = true;
                            Console.Write("\x1b[?7h");
                            Console.Write("\x1b[?1049l");
                            return logs;
                        }
                    }

                    continue;
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
