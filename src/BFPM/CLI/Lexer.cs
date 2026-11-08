using System.Collections.Generic;
using BFPM.CLI.Exceptions;

namespace BFPM.CLI;

public class Lexer
{
    public static List<Token> Tokenize(string command)
    {
        List<Token> tokens = [];
        string[] words = Utils.ParseWords(command);

        foreach (var word in words)
        {
            if (int.TryParse(word, out int id))
            {
                tokens.Add(new(TokenType.Integer, id));
                continue;
            }

            switch (word)
            {
                case "assign":
                    tokens.Add(new(TokenType.Assign));
                    break;
                
                case "clear":
                    tokens.Add(new(TokenType.Clear));
                    break;
                
                case "copy":
                    tokens.Add(new(TokenType.Copy));
                    break;
                
                case "current_user":
                    tokens.Add(new(TokenType.CurrentUser));
                    break;
                
                case "delete":
                    tokens.Add(new(TokenType.Delete));
                    break;
                
                case "edit":
                    tokens.Add(new(TokenType.Edit));
                    break;
                
                case "exit":
                    tokens.Add(new(TokenType.Exit));
                    break;
                
                case "folder":
                    tokens.Add(new(TokenType.Folder));
                    break;
                
                case "folders":
                    tokens.Add(new(TokenType.Folders));
                    break;

                case "help":
                    tokens.Add(new(TokenType.Help));
                    break;
                
                case "list":
                    tokens.Add(new(TokenType.List));
                    break;
                
                case "login":
                    tokens.Add(new(TokenType.Login));
                    break;
                
                case "logoff":
                    tokens.Add(new(TokenType.Logoff));
                    break;
                
                case "new":
                    tokens.Add(new(TokenType.New));
                    break;

                case "print":
                    tokens.Add(new(TokenType.Print));
                    break;
                
                case "project":
                    tokens.Add(new(TokenType.Project));
                    break;
                
                case "projects":
                    tokens.Add(new(TokenType.Projects));
                    break;
                
                case "register":
                    tokens.Add(new(TokenType.Register));
                    break;
                
                case "remove":
                    tokens.Add(new(TokenType.Remove));
                    break;
                
                case "settings":
                    tokens.Add(new(TokenType.Settings));
                    break;
                
                case "switch":
                    tokens.Add(new(TokenType.Switch));
                    break;
                
                case "tag":
                    tokens.Add(new(TokenType.Tag));
                    break;
                
                case "task":
                    tokens.Add(new(TokenType.Task));
                    break;
                
                case "upload":
                    tokens.Add(new(TokenType.Upload));
                    break;
                
                case "user":
                    tokens.Add(new(TokenType.User));
                    break;

                default:
                    throw new UnknownTokenException();
            }
        }

        tokens.Add(new(TokenType.End));
        return tokens;
    }
}
