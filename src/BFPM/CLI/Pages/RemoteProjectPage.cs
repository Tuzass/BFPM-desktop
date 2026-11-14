using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Enums;
using Infrastructure.Database;
using Infrastructure.Entities;
using Infrastructure.Enums;
using Workflow.Enums;
using Services;

namespace BFPM.CLI.Pages;

public class RemoteProjectPage
{
    public RemoteProjectPage()
    {
        
    }

    private static void PrintTitles(int firstRow, int projectID, int remoteID)
    {
        Console.SetCursorPosition(0, firstRow);
        Console.Write($"Project {projectID}");
        Console.SetCursorPosition(0, firstRow + 1);
        Console.Write($"Associated with remote project of ID {remoteID}");

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

    private static void PrintCopyKeyInstructions(int firstRow)
    {
        Console.SetCursorPosition(0, firstRow);
        Console.Write(@"Enter      | Exit & Create copy
ESC        | Exit without creating copy
Left/Right | Moves cursor");
    }

    private static void ClearCopyKeyInstructions(int firstRow)
    {
        Console.SetCursorPosition(0, firstRow);
        Console.Write(new string(' ', 100));

        Console.SetCursorPosition(0, firstRow + 1);
        Console.Write(new string(' ', 100));

        Console.SetCursorPosition(0, firstRow + 2);
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

    private static string DomainEntityStatusToString(DomainEntityStatus status)
    {
        return status switch
        {
            DomainEntityStatus.Active => "1",
            DomainEntityStatus.OnHold => "2",
            DomainEntityStatus.Archived => "3",
            DomainEntityStatus.Completed => "4",
            _ => "1"
        };
    }

    private static string ProjectStatusToString(ProjectStatus status)
    {
        return status switch
        {
            ProjectStatus.Active => "1",
            ProjectStatus.OnHold => "2",
            ProjectStatus.Archived => "3",
            ProjectStatus.Completed => "4",
            _ => "1"
        };
    }

    private static DomainEntityStatus StringToDomainEntityStatus(string statusString)
    {
        return statusString switch
        {
            "1" => DomainEntityStatus.Active,
            "2" => DomainEntityStatus.OnHold,
            "3" => DomainEntityStatus.Archived,
            "4" => DomainEntityStatus.Completed,
            _ => DomainEntityStatus.Active 
        };
    }

    public static async Task<List<string>> Run(AppDbContext context, ApiClient apiClient, int projectID)
    {
        var currentUser = await UserService.GetCurrentUser();
        var project = await DomainService.GetProjectByID(context, projectID);

        if (project.RemoteID is null)
        {
            throw new InvalidOperationException("Tried to run remote project page on local project.");
        }

        Change change = new(currentUser.ID, DomainEntityType.Project, projectID, null, null, null, null, null, false);
        List<string> logs = [];

        int titleFirstRow = 0;
        int fieldsFirstRow = 3;
        int mainOptionsFirstRow = 14;
        int confirmationMessageRow = 14;
        int keyInstructionsFirstRow = 14;
        int errorMessageRow = 21;

        List<string> fieldNames = ["Name", "Description", "Status"];
        List<string> originalFieldContents = [project.Name, project.Description, ProjectStatusToString(project.Status)];
        List<string> statusOptions = ["1", "2", "3", "4"];
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
        List<string> optionNames = ["Edit fields", "Commit changes", "Pull changes", "Create copy", "Exit"]; // TODO: add "Manage workflow rules"
        int optionCount = optionNames.Count;
        int optionIndex = 0;
        int cursorIndex;

        List<string> fieldContents = [project.Name, project.Description, ProjectStatusToString(project.Status)];
        var persistedChange = await DomainService.GetChange(context, DomainEntityType.Project, projectID);

        if (persistedChange is not null)
        {
            if (persistedChange.Name is not null)
            {
                fieldContents[0] = persistedChange.Name;
            }

            if (persistedChange.Description is not null)
            {
                fieldContents[1] = persistedChange.Description;
            }

            if (persistedChange.Status is not null)
            {
                fieldContents[2] = DomainEntityStatusToString(persistedChange.Status.Value);
            }
        }

        Console.Write("\x1b[?1049h");
        Console.Write("\x1b[?7l");
        Console.Clear();
        PrintTitles(titleFirstRow, projectID, project.RemoteID.Value);

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
                                PrintErrorMessage(errorMessageRow, "Please make sure the project's name has 3-50 characters and contains only letters, numbers, (, ), -, and spaces!");
                                Console.SetCursorPosition(cursorIndex, 10 + fieldIndex);
                                continue;
                            }

                            if (await DomainService.IsProjectNameDuplicate(context, currentUser.ID, fieldContents[0].Trim(), projectID))
                            {
                                PrintErrorMessage(errorMessageRow, $"Current user already has a project named '{fieldContents[0].Trim()}' (case-insensitive)!");
                                Console.SetCursorPosition(cursorIndex, 10 + fieldIndex);
                                continue;
                            }

                            if (!Utils.ValidateDomainEntityDescription(fieldContents[1].Trim()))
                            {
                                PrintErrorMessage(errorMessageRow, "Please make sure it is less than 2000 characters long!");
                                Console.SetCursorPosition(cursorIndex, 10 + fieldIndex);
                                continue;
                            }

                            if (!statusOptions.Contains(fieldContents[2].Trim()))
                            {
                                PrintErrorMessage(errorMessageRow, "Please choose a number from 1-4 for the project status!");
                                Console.SetCursorPosition(cursorIndex, 10 + fieldIndex);
                                continue;
                            }

                            int confirmationIndex = 0;
                            ClearEditingKeyInstructions(keyInstructionsFirstRow);
                            PrintConfirmationMessage(confirmationMessageRow, $"Do you wish to save your changes to project {projectID}'s attributes?");
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
                                    string newDescription = fieldContents[1].Trim();
                                    string newStatusString = fieldContents[2].Trim();

                                    DomainEntityStatus newStatus = StringToDomainEntityStatus(newStatusString);

                                    change.Name = newName == project.Name ? null : newName;
                                    change.Description = newDescription == project.Description ? null : newDescription;
                                    change.Status = newStatusString == ProjectStatusToString(project.Status) ? null : newStatus;
                                    await DomainService.StoreOrReplaceChange(context, change);
                                    await context.SaveChangesAsync();

                                    PrintErrorMessage(errorMessageRow, "Changes were saved. Select 'Commit changes' to submit them to the remote project.");
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
                    var result = await SynchronizationService.CommitChanges(context, apiClient,
                                                                            currentUser.ID,
                                                                            project.RemoteID!.Value,
                                                                            ActionType.ChangeAttribute,
                                                                            DomainEntityType.Project,
                                                                            projectID,
                                                                            null,
                                                                            null,
                                                                            change);

                    if (result)
                    {
                        PrintErrorMessage(errorMessageRow, "Changes were successfully commited to remote project! Applying changes to local project...");

                        await DomainService.ApplyChange(context, change);
                        await DomainService.DeleteStoredChange(context, currentUser.ID, DomainEntityType.Project, projectID);
                        await context.SaveChangesAsync();

                        ClearErrorMessage(errorMessageRow);
                        PrintErrorMessage(errorMessageRow, "Changes were successfully commited to remote project! Changes were also applied successfully to the associated local project! Other entities may have been affected, so make sure to perform synchronization.");
                        logs.Add("Changes were successfully commited to remote project! Changes were also applied successfully to the associated local project! Other entities may have been affected, so make sure to perform synchronization.");
                    }

                    else
                    {
                        PrintErrorMessage(errorMessageRow, "Changes were rejected by the server! Please perform synchronization on the entire project before submitting your changes again!");
                        logs.Add("Changes were rejected by the server! Please perform synchronization on the entire project before submitting your changes again!");
                    }
                }

                if (optionIndex == 2) // pull changes
                {
                    await SynchronizationService.PullChanges(context, apiClient, currentUser.ID, DomainEntityType.Project, projectID);
                    await context.SaveChangesAsync();
                    PrintErrorMessage(errorMessageRow, "Changes were successfully pulled!");
                    logs.Add("Changes were successfully pulled!");
                }

                if (optionIndex == optionCount - 2) // create copy
                {
                    if (!await DomainService.IsProjectAssignedToUser(context, currentUser.ID, project.ID!.Value))
                    {
                        PrintErrorMessage(errorMessageRow, "User is not authorized to copy this project.");
                        continue;
                    }

                    cursorIndex = 32;
                    string newName = "";
                    ClearMainOptions(mainOptionsFirstRow, optionNames);
                    ClearFields(fieldsFirstRow, fieldNames);

                    PrintCopyKeyInstructions(keyInstructionsFirstRow);
                    Console.SetCursorPosition(0, 3);
                    Console.Write("Select a new name for the copy: ");
                    Console.SetCursorPosition(32, 3);

                    bool shouldExit = false;
                    while (!shouldExit)
                    {
                        Console.CursorVisible = true;
                        var copyKey = Console.ReadKey(true);

                        if (copyKey.Key == ConsoleKey.Enter)
                        {
                            Console.CursorVisible = false;

                            if (!Utils.ValidateDomainEntityName(newName.Trim()))
                            {
                                PrintErrorMessage(errorMessageRow, "Please make sure the project's name has 3-50 characters and contains only letters, numbers, (, ), -, and spaces!");
                                continue;
                            }

                            if (await DomainService.IsProjectNameDuplicate(context, currentUser.ID, newName.Trim(), project.ID))
                            {
                                PrintErrorMessage(errorMessageRow, $"Current user already has a project named '{newName.Trim()}' (case-insensitive)!");
                                continue;
                            }

                            int confirmationIndex = 0;
                            ClearCopyKeyInstructions(keyInstructionsFirstRow);
                            PrintConfirmationMessage(confirmationMessageRow, "Do you wish to create a copy of the project with this name?");
                            PrintConfirmationOptions(confirmationMessageRow + 2, "Create copy", "Keep editing", confirmationIndex);

                            while (true)
                            {
                                var confirmationKey = Console.ReadKey(true);

                                if (confirmationKey.Key == ConsoleKey.LeftArrow)
                                {
                                    confirmationIndex = (confirmationIndex + 1) % 2;
                                    ClearConfirmationOptions(confirmationMessageRow + 2);
                                    PrintConfirmationOptions(confirmationMessageRow + 2, "Create copy", "Keep editing", confirmationIndex);
                                }
                                
                                if (confirmationKey.Key == ConsoleKey.RightArrow)
                                {
                                    confirmationIndex = (confirmationIndex + 1) % 2;
                                    ClearConfirmationOptions(confirmationMessageRow + 2);
                                    PrintConfirmationOptions(confirmationMessageRow + 2, "Create copy", "Keep editing", confirmationIndex);
                                }

                                if (confirmationKey.Key == ConsoleKey.Enter)
                                {
                                    if (confirmationIndex == 1)
                                    {
                                        PrintCopyKeyInstructions(keyInstructionsFirstRow);
                                        Console.SetCursorPosition(cursorIndex, 3);
                                        Console.CursorVisible = true;
                                        break;
                                    }

                                    ClearConfirmationOptions(confirmationMessageRow + 2);
                                    ClearConfirmationMessage(confirmationMessageRow);
                                    Console.SetCursorPosition(0, 3);
                                    Console.Write(new string(' ', 200));

                                    int copyID = await DomainService.CopyProject(context, currentUser.ID, project.ID!.Value, newName.Trim());
                                    await context.SaveChangesAsync();

                                    PrintErrorMessage(errorMessageRow, $"Project was successfully copied! Copy's ID: {copyID}.");
                                    logs.Add($"Project was successfully copied! Copy's ID: {copyID}.");

                                    shouldExit = true;
                                    break;
                                }
                            }
                        }

                        if (copyKey.Key == ConsoleKey.Escape)
                        {
                            int confirmationIndex = 0;
                            Console.CursorVisible = false;
                            ClearCopyKeyInstructions(keyInstructionsFirstRow);
                            PrintConfirmationMessage(confirmationMessageRow, "Are you sure you want to exit without copying the project?");
                            PrintConfirmationOptions(confirmationMessageRow + 2, "Yes", "No", confirmationIndex);

                            while (true)
                            {
                                var confirmationKey = Console.ReadKey(true);

                                if (confirmationKey.Key == ConsoleKey.LeftArrow)
                                {
                                    confirmationIndex = (confirmationIndex + 1) % 2;
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
                                    ClearConfirmationOptions(confirmationMessageRow + 2);
                                    ClearConfirmationMessage(confirmationMessageRow);
                                    Console.SetCursorPosition(0, 3);
                                    Console.Write(new string(' ', 200));

                                    if (confirmationIndex == 1)
                                    {
                                        break;
                                    }
                                    
                                    shouldExit = true;
                                    break;
                                }
                            }
                        }

                        if (copyKey.Key == ConsoleKey.LeftArrow)
                        {
                            if (cursorIndex == 32)
                            {
                                continue;
                            }

                            cursorIndex--;
                            Console.SetCursorPosition(cursorIndex, 3);
                            continue;
                        }

                        if (copyKey.Key == ConsoleKey.RightArrow)
                        {
                            if (cursorIndex == 32 + newName.Length)
                            {
                                continue;
                            }

                            cursorIndex++;
                            Console.SetCursorPosition(cursorIndex, 3);
                            continue;
                        }

                        if (copyKey.Key == ConsoleKey.Backspace)
                        {
                            if (cursorIndex == 32)
                            {
                                continue;
                            }

                            newName = newName.Remove(cursorIndex - 32 - 1, 1);
                            cursorIndex--;
                            Console.SetCursorPosition(32, 3);
                            Console.Write(newName + " ");
                            Console.SetCursorPosition(cursorIndex, 3);
                            continue;
                        }

                        if (!char.IsControl(copyKey.KeyChar)
                         && (char.IsLetterOrDigit(copyKey.KeyChar)
                          || char.IsPunctuation(copyKey.KeyChar)
                          || char.IsWhiteSpace(copyKey.KeyChar)))
                        {
                            newName = newName.Insert(cursorIndex - 32, copyKey.KeyChar.ToString());
                            cursorIndex++;
                            Console.SetCursorPosition(32, 3);
                            Console.Write(newName);
                            Console.SetCursorPosition(cursorIndex, 3);
                        }

                        continue;
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
