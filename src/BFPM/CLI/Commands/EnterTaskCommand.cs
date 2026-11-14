using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Database;
using Services;
using BFPM.CLI.Exceptions;
using BFPM.CLI.Pages;

namespace BFPM.CLI.Commands;

public class EnterTaskCommand : ICommand
{
    private readonly ApiClient _apiClient = null!;
    public int TaskID {get; private set;}

    public EnterTaskCommand(ApiClient apiClient, int taskID)
    {
        _apiClient = apiClient;
        TaskID = taskID;
    }

    private EnterTaskCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        using (var context = AppDbContextFactory.CreateContext())
        {
            var task = await DomainService.GetTaskByID(context, TaskID);

            if (!await DomainService.IsProjectAssignedToUser(context, currentUser.ID, task.ProjectID))
            {
                throw new Services.Exceptions.UnauthorizedAccessException();
            }

            List<string> logs = [];

            if (await DomainService.IsProjectLocal(context, task.ProjectID))
            {
                logs = await LocalTaskPage.Run(context, _apiClient, TaskID);
            }

            else
            {
                logs = await RemoteTaskPage.Run(context, _apiClient, TaskID);
            }

            foreach (var log in logs)
            {
                Console.WriteLine(log);
            }
        }
    }
}
