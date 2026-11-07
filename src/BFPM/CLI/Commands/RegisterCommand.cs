using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BFPM.CLI.Pages;
using Services;
using Services.Exceptions;

namespace BFPM.CLI.Commands;

public class RegisterCommand : ICommand
{
    private readonly ApiClient _apiClient = null!;

    public RegisterCommand(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    private RegisterCommand()
    {
        
    }

    public async Task Execute()
    {
        List<string>? fieldContents = ["", "", ""];
        string? errorMessage = null;

        while (true)
        {
            try
            {
                fieldContents = await RegisterPage.Run(fieldContents!, errorMessage);

                if (fieldContents is null || fieldContents.Count < 3)
                {
                    return;
                }

                if (await _apiClient.Register(fieldContents[0], fieldContents[1], fieldContents[2]))
                {
                    Console.WriteLine("New user registered successfully!");

                    if (!await _apiClient.LoginWithCredentials(fieldContents[0], fieldContents[2]))
                    {
                        Console.WriteLine("Login as new user failed!");
                    }

                    return;
                }

                errorMessage = "There was an error registering the user.";
            }

            catch (Exception e)
            {
                errorMessage = e switch
                {
                    ServerUnavailableException or DuplicateUserNameException or DuplicateEmailException => e.Message,
                    _ => throw new Exception(e.Message),
                };
            }
        }
    }
}
