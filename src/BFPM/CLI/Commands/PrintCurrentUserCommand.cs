using System;
using System.Threading.Tasks;
using Services;
using Services.Exceptions;

namespace BFPM.CLI.Commands;

public class PrintCurrentUserCommand : ICommand
{
    public PrintCurrentUserCommand()
    {
        
    }

    public async Task Execute()
    {
        try
        {
            var currentUser = await UserService.GetCurrentUser();
            Console.WriteLine(Utils.UserToString(currentUser));
        }
        
        catch (NoCurrentUserException)
        {
            Console.WriteLine("No user is currently selected");
        }
    }
}
