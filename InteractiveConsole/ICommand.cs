using System.Threading.Tasks;

namespace InteractiveConsole
{
    public interface ICommand
    {
        object Execute();
    }
}