using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Domain.Entities;

namespace BFPM;

public partial class Utils
{
    [GeneratedRegex(@"^[\p{L}\p{Nd}\s()\-]+$")]
    private static partial Regex DomainEntityNameRegex();

    public static string[] ParseWords(string inputString)
    {
        string[] input = inputString.Split(' ');
        
        for (int i = 0; i < input.Length - 1; i++)
        {
            input[i] = input[i].Trim([' ', '\n', '\t']);
            input[i] = input[i].ToLowerInvariant();
        }

        return input;
    }

    public static string UserToString(User user)
    {
        return $@"ID: {user.ID}
Username: {user.Username}
Email: {user.Email}";
    }

    public static string FolderToString(Folder folder)
    {
        return $@"ID: {folder.ID}
Name: {folder.Name}
Description: {folder.Description}";
    }

    public static string ProjectToString(Project project)
    {
        return $@"ID: {project.ID}
Name: {project.Name}
Description: {project.Description}
Status: {project.Status}
Created at: {project.CreatedAt}
Completed at: {project.CompletedAt}";
    }

    public static string ProjectListToString(List<Project> projects)
    {
        List<string> projectStrings = [];

        foreach (var project in projects)
        {
            projectStrings.Add($"{project.Name} ({project.ID})");
        }

        return string.Join(", ", projectStrings);
    }

    public static string TaskToString(TaskItem task)
    {
        return $@"ID: {task.ID}
Name: {task.Name}
Description: {task.Description}
Status: {task.Status}
Priority: {task.Priority}
DueDate: {task.DueDate}
Created at: {task.CreatedAt}
Completed at: {task.CompletedAt}";
    }

    public static string TaskListToString(List<TaskItem> tasks)
    {
        List<string> taskStrings = [];

        foreach (var task in tasks)
        {
            taskStrings.Add($"{task.Name} ({task.ID})");
        }

        return string.Join(", ", taskStrings);
    }

    public static string TagToString(Tag tag)
    {
        return $@"ID: {tag.ID}
Name: {tag.Name}";
    }

    public static string TagListToString(List<Tag> tags)
    {
        List<string> tagStrings = [];

        foreach (var tag in tags)
        {
            tagStrings.Add($"{tag.Name} ({tag.ID})");
        }

        return string.Join(", ", tagStrings);
    }

    public static bool ValidateDomainEntityName(string name)
    {
        if (name.Length < 3 || name.Length > 50)
        {
            return false;
        }

        return DomainEntityNameRegex().IsMatch(name);
    }

    public static bool ValidateDomainEntityDescription(string description)
    {
        return description.Length < 2000;
    }

    public static bool ValidateUserName(string userName)
    {
        return userName.Length >= 3 && userName.Length <= 24;
    }

    public static bool ValidateEmail(string email)
    {
        try
        {
            var address = new MailAddress(email);
            return address.Address == email;
        }

        catch (FormatException)
        {
            return false;
        }
    }

    public static bool ValidatePassword(string password)
    {
        return password.Length >= 8 && password.Length <= 24;
    }

    public static bool GetYesNoConfirmationCli(string question)
    {
        List<string> validAnswers = ["Y", "N", "y", "n"];
        Console.Write(question + " (Y/N)");
        string? answer = Console.ReadLine();

        while (answer is null || !validAnswers.Contains(answer))
        {
            answer = Console.ReadLine();
        }

        if (answer == "Y" || answer == "y")
        {
            return true;
        }

        return false;
    }
}
