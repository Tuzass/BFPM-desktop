using System.Threading.Tasks;
using Infrastructure.Database;
using Services;
using BFPM.CLI.Exceptions;
using System;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

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
            
            string originalName = project.Name;
            int remoteID = await _apiClient.UploadProject(project.Name, project.Description, project.Status, tasks, tags, taskTagLinks);

            try
            {
                int remoteCopyID = await DomainService.CopyProject(context, currentUser.ID, ProjectID, remoteID);

                Change change = new(Infrastructure.Enums.DomainEntityType.Project, remoteCopyID, " ", null, null, null, null, false);
                await DomainService.ApplyChange(context, change);

                change = new(Infrastructure.Enums.DomainEntityType.Project, ProjectID, originalName + " - Local copy", null, null, null, null, false);
                await DomainService.ApplyChange(context, change);

                change = new(Infrastructure.Enums.DomainEntityType.Project, remoteCopyID, originalName, null, null, null, null, false);
                await DomainService.ApplyChange(context, change);

                Console.WriteLine($"Project {originalName} (ID: {ProjectID}) successfully uploaded!");
                Console.WriteLine($"Project with ID {remoteCopyID} is now associated to the remote copy!");
            }

            catch (DbUpdateException e)
            {
                Console.WriteLine(e.InnerException?.Message);
                throw;
            }
        }
    }
}
