using System;
using System.Threading.Tasks;
using API.Requests;
using Infrastructure.Database;
using Services;
using Services.Exceptions;

namespace BFPM.CLI.Commands;

public class DeleteProjectCommand : ICommand
{
    private readonly ApiClient _apiClient = null!;
    public int ProjectID {get; private set;}

    public DeleteProjectCommand(ApiClient apiClient, int projectID)
    {
        _apiClient = apiClient;
        ProjectID = projectID;
    }

    private DeleteProjectCommand()
    {
        
    }

    public async Task Execute()
    {
        var currentUser = await UserService.GetCurrentUser();

        try
        {
            using (var context = AppDbContextFactory.CreateContext())
            {
                if (!await DomainService.IsProjectOwnedByUser(context, currentUser.ID, ProjectID))
                {
                    Console.WriteLine("You don't have permission to delete this project.");
                    return;
                }

                if (!await DomainService.IsProjectLocal(context, ProjectID))
                {
                    bool answer = Utils.GetYesNoConfirmationCli($"This project is associated with a remote project. You can only delete this while deleting the remote project as well. Would you like to proceed?");

                    if (!answer)
                    {
                        return;
                    }

                    var result = await SynchronizationService.DeleteProject(context, _apiClient, currentUser.ID, ProjectID);

                    if (!result)
                    {
                        Console.WriteLine("The remote project could not be deleted.");
                        return;
                    }
                }

                await DomainService.DeleteProject(context, ProjectID);
                await context.SaveChangesAsync();
            }
        }

        catch (Services.Exceptions.UnauthorizedAccessException)
        {
            Console.WriteLine("You don't have permission to delete this project.");
        }

        catch (ServerUnavailableException)
        {
            Console.WriteLine("The server is currently unavailable, please try again later.");
        }

        catch (InvalidOperationException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
