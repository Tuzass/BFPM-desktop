namespace BFPM.CLI;

public enum TokenType
{
    Integer,
    Assign,
    Clear,
    CurrentUser,
    Delete,
    Enter,
    End,
    Exit,
    Folder,
    Folders,
    Help,
    List,
    Login,
    Logoff,
    New,
    Print,
    Project,
    Projects,
    Register,
    Remove,
    Settings,
    Switch,
    Tag,
    Task,
    Upload,
    User
}

public class Token
{
    public TokenType Type {get; private set;}
    public int? Value {get; private set;}

    public Token(TokenType type, int? value = null)
    {
        Type = type;
        Value = value;
    }

    private Token()
    {
        
    }
}
