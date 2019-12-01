using InteractiveConsole.Attributes;
using InteractiveConsole.Storage;

namespace InteractiveConsole.Commands
{
    [Command]
    public class PrintVariablesCommand : BaseCommand, ICommand
    {
        private readonly IInMemoryStorage _inMemoryStorage;
        public PrintVariablesCommand(IInMemoryStorage inMemoryStorage)
        {
            _inMemoryStorage = inMemoryStorage;
        }

        public object Execute()
        {
            Printer.PrintHeader($"Storage contains {_inMemoryStorage.Variables.Count} variables.");

            foreach (var variable in _inMemoryStorage.Variables)
            {
                Printer.Print($"{variable.Id}: {variable.Description}");
            }

            Printer.Print();

            return null;
        }
    }
}