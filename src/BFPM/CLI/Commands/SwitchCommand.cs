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
        var users = await UserService.GetStoredRegisteredUsers();
        var localUser = await UserService.GetLocalUser();
        users.Insert(0, localUser);
        string? errorMessage = null;

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

                KeyValuePair<string, string>? credentials = new("", "");
                while (true)
                {
                    credentials = LoginPage.Run(credentials.Value.Key, credentials.Value.Value);

                    if (credentials is null)
                    {
                        break;
                    }

                    if (await _apiClient.LoginWithCredentials(credentials.Value.Key, credentials.Value.Value))
                    {
                        Console.WriteLine($"Successfully logged in as: {credentials.Value.Key}");
                        return;
                    }
                    Console.WriteLine("Invalid credentials!");
                }
            }

            catch (ServerUnavailableException e)
            {
                errorMessage = e.Message;
            }
        }
    }
}
