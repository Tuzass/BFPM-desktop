using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Database;
using Services;
using Services.Exceptions;
using BFPM.CLI.Exceptions;
using API.Requests;
using System;

namespace BFPM.CLI.Commands;

public class UploadProjectCommand : ICommand
{
    private readonly ApiClient _apiClient = null!;
    public int ProjectID {get; private set;}

    public UploadProjectCommand(ApiClient apiClient, int projectID)
    {
        _apiClient = apiClient;
        ProjectID = projectID;
    }

    private UploadProjectCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        using (var context = AppDbContextFactory.CreateContext())
        {
            if (!await DomainService.IsProjectOwnedByUser(context, currentUser.ID, ProjectID))
            {
                throw new Services.Exceptions.UnauthorizedAccessException();
            }

            if (!await DomainService.IsProjectLocal(context, ProjectID))
            {
                throw new NonlocalChangeException();
            }

            var project = await DomainService.GetProjectByID(context, ProjectID);
            var tasks = await DomainService.GetTasksByProject(context, ProjectID);
            var tags = await DomainService.GetTagsByProject(context, ProjectID);
            var taskTagLinks = await DomainService.GetTaskTagLinksByProject(context, ProjectID);
                                        
            await _apiClient.UploadProject(project.Name, project.Description, project.Status, tasks, tags, taskTagLinks);
            Console.WriteLine($"Project {project.Name} (ID: {ProjectID}) successfully uploaded!");

            // prompt the user about keeping the local copy that was uploaded or not
        }
    }
}
