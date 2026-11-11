using System;
using System.Threading.Tasks;
using Services;
using Infrastructure.Database;
using BFPM.CLI.Pages;

namespace BFPM.CLI.Commands;

public class EnterProjectCommand : ICommand
{
    private readonly ApiClient _apiClient = null!;
    public int ProjectID {get; private set;}

    public EnterProjectCommand(ApiClient apiClient, int projectID)
    {
        _apiClient = apiClient;
        ProjectID = projectID;
    }

    private EnterProjectCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        using (var context = AppDbContextFactory.CreateContext())
        {
            if (!await DomainService.IsProjectAssignedToUser(context, currentUser.ID, ProjectID))
            {
                throw new Services.Exceptions.UnauthorizedAccessException();
            }

            var logs = await ProjectPage.Run(context, _apiClient, ProjectID);

            foreach (var log in logs)
            {
                Console.WriteLine(log);
            }
        }
    }
}
