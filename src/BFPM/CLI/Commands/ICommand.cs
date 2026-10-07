using System.Threading.Tasks;

namespace BFPM.CLI.Commands;

public interface ICommand
{
    Task Execute();
}
