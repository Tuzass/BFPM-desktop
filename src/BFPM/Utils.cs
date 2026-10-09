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

    public static string TagToString(Tag tag)
    {
        return $@"ID: {tag.ID}
Name: {tag.Name}";
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
}
