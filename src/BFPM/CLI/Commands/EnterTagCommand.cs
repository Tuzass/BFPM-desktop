using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Database;
using Services;
using BFPM.CLI.Exceptions;
using BFPM.CLI.Pages;

namespace BFPM.CLI.Commands;

public class EnterTagCommand : ICommand
{
    private readonly ApiClient _apiClient = null!;
    public int TagID {get; private set;}

    public EnterTagCommand(ApiClient apiClient, int tagID)
    {
        _apiClient = apiClient;
        TagID = tagID;
    }

    private EnterTagCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        using (var context = AppDbContextFactory.CreateContext())
        {
            var tag = await DomainService.GetTagByID(context, TagID);

            if (!await DomainService.IsProjectAssignedToUser(context, currentUser.ID, tag.ProjectID))
            {
                throw new Services.Exceptions.UnauthorizedAccessException();
            }

            List<string> logs = [];

            if (await DomainService.IsProjectLocal(context, tag.ProjectID))
            {
                logs = await LocalTagPage.Run(context, TagID);
            }

            else
            {
                logs = await RemoteTagPage.Run(context, _apiClient, TagID);
            }

            foreach (var log in logs)
            {
                Console.WriteLine(log);
            }
        }
    }
}
