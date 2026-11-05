using System.Threading.Tasks;
using Services;

namespace BFPM.CLI.Commands;

public class LogoffCommand : ICommand
{
    private readonly ApiClient _apiClient = null!;

    public LogoffCommand(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    private LogoffCommand()
    {
        
    }

    public async Task Execute()
    {
        await _apiClient.Logout();
    }
}
