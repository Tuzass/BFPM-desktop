using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services;
using Services.Exceptions;
using BFPM.CLI.Pages;

namespace BFPM.CLI.Commands;

public class LoginCommand : ICommand
{
    private readonly ApiClient _apiClient = null!;

    public LoginCommand(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    private LoginCommand()
    {
        
    }

    public async Task Execute()
    {
        List<string>? fieldContents = ["", ""];
        string? errorMessage = null;

        while (true)
        {
            try
            {
                fieldContents = LoginPage.Run(fieldContents!, errorMessage);

                if (fieldContents is null || fieldContents.Count < 2)
                {
                    return;
                }

                if (await _apiClient.LoginWithCredentials(fieldContents[0], fieldContents[1]))
                {
                    Console.WriteLine($"Successfully logged in as {fieldContents[0]}!");
                    return;
                }

                errorMessage = "There was an error while logging in.";
            }

            catch (Exception e)
            {
                errorMessage = e switch
                {
                    ServerUnavailableException => e.Message,
                    InexistentUserException => $"The user '{fieldContents![0]}' doesn't exist.",
                    _ => throw new Exception(e.Message),
                };
            }
        }
    }
}
