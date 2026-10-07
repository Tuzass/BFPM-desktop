using System.Threading.Tasks;
using Services;

namespace BFPM.CLI.Commands;

public class RemoveUserCommand : ICommand
{
    public int UserID {get; private set;}

    public RemoveUserCommand(int folderID)
    {
        UserID = folderID;
    }

    private RemoveUserCommand()
    {
        
    }

    public async Task Execute()
    {
        await UserService.RemoveRegisteredUser(UserID);
    }
}
