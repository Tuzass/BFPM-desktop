using System;
using System.Threading.Tasks;

namespace BFPM.CLI.Commands;

public class ClearCommand : ICommand
{
    public ClearCommand()
    {
        
    }

    public Task Execute()
    {
        Console.Clear();
        return Task.CompletedTask;
    }
}
