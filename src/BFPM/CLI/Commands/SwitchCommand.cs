using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Services;
using Services.Exceptions;
using BFPM.CLI.Pages;

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
        users.Add(await UserService.GetLocalUser());

        while (true)
        {
            var selectedUserID = await SwitchPage.Run(users);

            if (selectedUserID is null)
            {
                return;
            }

            if (selectedUserID == 0)
            {
                await UserService.SwitchCurrentUser(0);
                return;
            }

            if (await _apiClient.LoginWithPersistentToken(selectedUserID.Value))
            {
                return;
            }

            var user = users.SingleOrDefault(u => u.ID == selectedUserID);

            if (user is null)
            {
                throw new InexistentUserException();
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
                    break;
                }
            }
        }
    }
}
