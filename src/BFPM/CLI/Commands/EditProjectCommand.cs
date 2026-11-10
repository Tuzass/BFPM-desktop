using System;
using System.Threading.Tasks;
using Services;
using Infrastructure.Database;
using BFPM.CLI.Pages;
using BFPM.CLI.Exceptions;

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

            if (!await DomainService.IsProjectLocal(context, ProjectID))
            {
                throw new NonlocalProjectException();
            }

            var logs = await ProjectEditPage.Run(context, _apiClient, ProjectID);

            foreach (var log in logs)
            {
                Console.WriteLine(log);
            }
        }
    }
}
