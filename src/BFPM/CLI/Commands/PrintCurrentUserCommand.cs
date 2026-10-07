using System;
using System.Threading.Tasks;
using Services;

namespace BFPM.CLI.Commands;

public class PrintCurrentUserCommand : ICommand
{
    public PrintCurrentUserCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();
        Console.WriteLine(Utils.UserToString(currentUser));
    }
}
