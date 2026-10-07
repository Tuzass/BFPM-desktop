using System.Linq;
using System.Collections.Generic;
using BFPM.CLI.Commands;
using BFPM.CLI.Exceptions;

namespace BFPM.CLI;

public class Parser
{
    private readonly static TokenType[] ValidHelpCommands = [TokenType.Assign, TokenType.Clear, TokenType.Copy, TokenType.Delete, TokenType.Edit, TokenType.Exit, TokenType.List, TokenType.Login, TokenType.Logoff, TokenType.New, TokenType.Print, TokenType.Register, TokenType.Remove, TokenType.Switch];

    public static ICommand? Parse(List<Token> tokens)
    {
        var first = tokens[0];

        switch (first.Type)
        {
            case TokenType.Integer:
                throw new UnexpectedTokenException();
            
            case TokenType.Assign:
                return ParseAssign(tokens[1..]);
            
            case TokenType.Clear:
                if (tokens[1].Type == TokenType.End)
                {
                    return new ClearCommand();
                }

                throw new UnexpectedTokenException();
            
            case TokenType.Copy:
                return ParseCopy(tokens[1..]);
            
            case TokenType.Delete:
                return ParseDelete(tokens[1..]);
            
            case TokenType.Edit:
                return ParseEdit(tokens[1..]);
            
            case TokenType.Exit:
                return null;
            
            case TokenType.Help:
                return ParseHelp(tokens[1..]);
            
            case TokenType.List:
                return ParseList(tokens[1..]);
            
            case TokenType.Login:
                if (tokens[1].Type == TokenType.End)
                {
                    return new LoginCommand();
                }

                throw new UnexpectedTokenException();
            
            case TokenType.Logoff:
                if (tokens[1].Type == TokenType.End)
                {
                    return new LogoffCommand();
                }

                throw new UnexpectedTokenException();
            
            case TokenType.New:
                return ParseNew(tokens[1..]);
            
            case TokenType.Print:
                return ParsePrint(tokens[1..]);
            
            case TokenType.Register:
                if (tokens[1].Type == TokenType.End)
                {
                    return new RegisterCommand();
                }

                throw new UnexpectedTokenException();
            
            case TokenType.Remove:
                return ParseRemove(tokens[1..]);
            
            case TokenType.Switch:
                if (tokens[1].Type == TokenType.End)
                {
                    return new SwitchCommand();
                }

                throw new UnexpectedTokenException();
        }

        return null;
    }

    public static ICommand ParseAssign(List<Token> tokens)
    {
        if (tokens.Count < 5)
        {
            throw new UnexpectedTokenException();
        }

        if (tokens[0].Type == TokenType.Project
         && tokens[1].Type == TokenType.Integer
         && tokens[2].Type == TokenType.Folder
         && tokens[3].Type == TokenType.Integer
         && tokens[4].Type == TokenType.End)
        {
            if (tokens[1].Value is not int projectID || tokens[3].Value is not int folderID)
            {
                throw new InvalidIdException();
            }

            return new AssignProjectToFolderCommand(folderID, projectID);
        }

        if (tokens[0].Type == TokenType.Tag
         && tokens[1].Type == TokenType.Integer
         && tokens[2].Type == TokenType.Task
         && tokens[3].Type == TokenType.Integer
         && tokens[4].Type == TokenType.End)
        {
            if (tokens[1].Value is not int tagID || tokens[3].Value is not int taskID)
            {
                throw new InvalidIdException();
            }

            return new AssignTagToTaskCommand(taskID, tagID);
        }

        throw new UnexpectedTokenException();
    }

    public static ICommand ParseCopy(List<Token> tokens)
    {
        if (tokens.Count < 2)
        {
            throw new UnexpectedTokenException();
        }

        if (tokens[0].Type == TokenType.Integer
         && tokens[1].Type == TokenType.End)
        {
            if (tokens[0].Value is not int projectID)
            {
                throw new InvalidIdException();
            }

            return new CopyProjectCommand(projectID);
        }

        throw new UnexpectedTokenException();
    }

    public static ICommand ParseDelete(List<Token> tokens)
    {
        if (tokens.Count < 3)
        {
            throw new UnexpectedTokenException();
        }

        if (tokens[0].Type == TokenType.Folder
         && tokens[1].Type == TokenType.Integer
         && tokens[2].Type == TokenType.End)
        {
            if (tokens[1].Value is not int folderID)
            {
                throw new InvalidIdException();
            }

            return new DeleteFolderCommand(folderID);
        }

        if (tokens[0].Type == TokenType.Project
         && tokens[1].Type == TokenType.Integer
         && tokens[2].Type == TokenType.End)
        {
            if (tokens[1].Value is not int projectID)
            {
                throw new InvalidIdException();
            }

            return new DeleteProjectCommand(projectID);
        }

        if (tokens[0].Type == TokenType.Task
         && tokens[1].Type == TokenType.Integer
         && tokens[2].Type == TokenType.End)
        {
            if (tokens[1].Value is not int taskID)
            {
                throw new InvalidIdException();
            }

            return new DeleteTaskCommand(taskID);
        }

        if (tokens[0].Type == TokenType.Tag
         && tokens[1].Type == TokenType.Integer
         && tokens[2].Type == TokenType.End)
        {
            if (tokens[1].Value is not int tagID)
            {
                throw new InvalidIdException();
            }

            return new DeleteTagCommand(tagID);
        }

        if (tokens[0].Type == TokenType.User
         && tokens[1].Type == TokenType.Integer
         && tokens[2].Type == TokenType.End)
        {
            if (tokens[1].Value is not int userID)
            {
                throw new InvalidIdException();
            }

            return new RemoveUserCommand(userID);
        }

        throw new UnexpectedTokenException();
    }

    public static ICommand ParseEdit(List<Token> tokens)
    {
        if (tokens.Count < 2)
        {
            throw new UnexpectedTokenException();
        }

        if (tokens[0].Type == TokenType.CurrentUser
         && tokens[1].Type == TokenType.End)
        {
            return new EditCurrentUserCommand();
        }

        if (tokens[0].Type == TokenType.Settings
         && tokens[1].Type == TokenType.End)
        {
            return new EditSettingsCommand();
        }

        if (tokens.Count < 3)
        {
            throw new UnexpectedTokenException();
        }

        if (tokens[0].Type == TokenType.Folder
         && tokens[1].Type == TokenType.Integer
         && tokens[2].Type == TokenType.End)
        {
            if (tokens[1].Value is not int folderID)
            {
                throw new InvalidIdException();
            }

            return new EditFolderCommand(folderID);
        }

        if (tokens[0].Type == TokenType.Project
         && tokens[1].Type == TokenType.Integer
         && tokens[2].Type == TokenType.End)
        {
            if (tokens[1].Value is not int projectID)
            {
                throw new InvalidIdException();
            }

            return new EditProjectCommand(projectID);
        }

        if (tokens[0].Type == TokenType.Task
         && tokens[1].Type == TokenType.Integer
         && tokens[2].Type == TokenType.End)
        {
            if (tokens[1].Value is not int taskID)
            {
                throw new InvalidIdException();
            }

            return new EditTaskCommand(taskID);
        }

        if (tokens[0].Type == TokenType.Tag
         && tokens[1].Type == TokenType.Integer
         && tokens[2].Type == TokenType.End)
        {
            if (tokens[1].Value is not int tagID)
            {
                throw new InvalidIdException();
            }

            return new EditTagCommand(tagID);
        }

        throw new UnexpectedTokenException();
    }

    public static ICommand ParseHelp(List<Token> tokens)
    {
        if (tokens[0].Type == TokenType.End)
        {
            return new HelpCommand(null);
        }

        if (ValidHelpCommands.Contains(tokens[0].Type)
         && tokens[1].Type == TokenType.End)
        {
            string commandString = tokens[0].Type.ToString().ToLowerInvariant();
            return new HelpCommand(commandString);
        }

        throw new UnexpectedTokenException();
    }

    public static ICommand ParseList(List<Token> tokens)
    {
        if (tokens.Count < 2)
        {
            throw new UnexpectedTokenException();
        }

        if (tokens[0].Type == TokenType.Folders
         && tokens[1].Type == TokenType.End)
        {
            return new ListFoldersCommand();
        }

        if (tokens[0].Type == TokenType.Projects
         && tokens[1].Type == TokenType.End)
        {
            return new ListProjectsCommand();
        }

        if (tokens[0].Type == TokenType.Settings
         && tokens[1].Type == TokenType.End)
        {
            return new ListSettingsCommand();
        }

        throw new UnexpectedTokenException();
    }

    public static ICommand ParseNew(List<Token> tokens)
    {
        if (tokens.Count < 2)
        {
            throw new UnexpectedTokenException();
        }

        if (tokens[0].Type == TokenType.Folder
         && tokens[1].Type == TokenType.End)
        {
            return new NewFolderCommand();
        }

        if (tokens[0].Type == TokenType.Project
         && tokens[1].Type == TokenType.End)
        {
            return new NewProjectCommand();
        }

        if (tokens.Count < 3)
        {
            throw new UnexpectedTokenException();
        }

        if (tokens[0].Type == TokenType.Task
         && tokens[1].Type == TokenType.Integer
         && tokens[2].Type == TokenType.End)
        {
            if (tokens[1].Value is not int projectID)
            {
                throw new InvalidIdException();
            }

            return new NewTaskCommand(projectID);
        }

        if (tokens[0].Type == TokenType.Tag
         && tokens[1].Type == TokenType.Integer
         && tokens[2].Type == TokenType.End)
        {
            if (tokens[1].Value is not int projectID)
            {
                throw new InvalidIdException();
            }

            return new NewTagCommand(projectID);
        }

        throw new UnexpectedTokenException();
    }

    public static ICommand ParsePrint(List<Token> tokens)
    {
        if (tokens.Count < 2)
        {
            throw new UnexpectedTokenException();
        }

        if (tokens[0].Type == TokenType.CurrentUser
         && tokens[1].Type == TokenType.End)
        {
            return new PrintCurrentUserCommand();
        }

        if (tokens.Count < 3)
        {
            throw new UnexpectedTokenException();
        }

        if (tokens[0].Type == TokenType.Folder
         && tokens[1].Type == TokenType.Integer
         && tokens[2].Type == TokenType.End)
        {
            if (tokens[1].Value is not int folderID)
            {
                throw new InvalidIdException();
            }

            return new PrintFolderCommand(folderID);
        }

        if (tokens[0].Type == TokenType.Project
         && tokens[1].Type == TokenType.Integer
         && tokens[2].Type == TokenType.End)
        {
            if (tokens[1].Value is not int projectID)
            {
                throw new InvalidIdException();
            }

            return new PrintProjectCommand(projectID);
        }

        if (tokens[0].Type == TokenType.Task
         && tokens[1].Type == TokenType.Integer
         && tokens[2].Type == TokenType.End)
        {
            if (tokens[1].Value is not int taskID)
            {
                throw new InvalidIdException();
            }

            return new PrintTaskCommand(taskID);
        }

        if (tokens[0].Type == TokenType.Tag
         && tokens[1].Type == TokenType.Integer
         && tokens[2].Type == TokenType.End)
        {
            if (tokens[1].Value is not int tagID)
            {
                throw new InvalidIdException();
            }

            return new PrintTagCommand(tagID);
        }

        throw new UnexpectedTokenException();
    }

    public static ICommand ParseRemove(List<Token> tokens)
    {
        if (tokens.Count < 5)
        {
            throw new UnexpectedTokenException();
        }

        if (tokens[0].Type == TokenType.Project
         && tokens[1].Type == TokenType.Integer
         && tokens[2].Type == TokenType.Folder
         && tokens[3].Type == TokenType.Integer
         && tokens[4].Type == TokenType.End)
        {
            if (tokens[1].Value is not int projectID || tokens[3].Value is not int folderID)
            {
                throw new InvalidIdException();
            }

            return new RemoveProjectFromFolderCommand(folderID, projectID);
        }

        if (tokens[0].Type == TokenType.Tag
         && tokens[1].Type == TokenType.Integer
         && tokens[2].Type == TokenType.Task
         && tokens[3].Type == TokenType.Integer
         && tokens[4].Type == TokenType.End)
        {
            if (tokens[1].Value is not int tagID || tokens[3].Value is not int taskID)
            {
                throw new InvalidIdException();
            }

            return new RemoveTagFromTaskCommand(taskID, tagID);
        }

        throw new UnexpectedTokenException();
    }
}
