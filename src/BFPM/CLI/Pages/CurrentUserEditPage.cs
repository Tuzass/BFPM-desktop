using System;
using System.Threading.Tasks;
using Services;
using Services.Exceptions;

namespace BFPM.CLI.Pages;

public class CurrentUserEditPage
{
    public CurrentUserEditPage()
    {
        
    }

    public static async Task Run()
    {
        var currentUser = await UserService.GetCurrentUser();

        if (currentUser is null)
        {
            throw new NoCurrentUserException();
        }

        if (currentUser.ID == 0)
        {
            Console.WriteLine("Local User (ID: 0) can't be edited!");
        }

        // TODO
        // implement alternate terminal screen where editing happens
        // check for duplicate username and email
        // save changes
    }
}
