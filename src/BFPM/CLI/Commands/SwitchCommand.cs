using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Services;
using BFPM.CLI.Pages;
using System;
using Services.Exceptions;

namespace BFPM.CLI.Commands;

public class SwitchCommand : ICommand
{
    private readonly ApiClient _apiClient = null!;

    public SwitchCommand(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    private SwitchCommand()
    {
        
    }

    public async Task Execute()
    {
        var users = await UserService.GetStoredUsers();
        var localUser = await UserService.GetLocalUser();
        users.Insert(0, localUser);
        string? errorMessage = null;

        try
        {
            var currentUser = await UserService.GetCurrentUser();
            currentUser = users.Single(u => u.ID == currentUser.ID);
            users.Remove(currentUser);
        }

        catch (NoCurrentUserException)
        {
            
        }

        while (true)
        {
            var selectedUserID = await SwitchPage.Run(users, errorMessage);

            if (selectedUserID is null)
            {
                return;
            }

            var user = users.SingleOrDefault(u => u.ID == selectedUserID);

            if (user is null)
            {
                errorMessage = "There was an unexpected error; please select a user again.";
                continue;
            }

            if (selectedUserID == 0)
            {
                await UserService.SwitchCurrentUser(0);
                Console.WriteLine($"Successfully logged in as: {user.Username}");
                return;
            }

            try
            {
                if (await _apiClient.LoginWithPersistentToken(selectedUserID.Value))
                {
                    Console.WriteLine($"Successfully logged in as: {user.Username}");
                    return;
                }

                List<string>? credentials = [user.Username, ""];
                while (true)
                {
                    credentials = LoginPage.Run(credentials, null);

                    if (credentials is null || credentials.Count < 2)
                    {
                        break;
                    }

                    if (await _apiClient.LoginWithCredentials(credentials[0], credentials[1]))
                    {
                        Console.WriteLine($"Successfully logged in as: {credentials[0]}");
                        return;
                    }

                    errorMessage = "Invalid credentials!";
                }
            }

            catch (ServerUnavailableException e)
            {
                errorMessage = e.Message;
            }
        }
    }
}
