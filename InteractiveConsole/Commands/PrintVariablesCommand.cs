using InteractiveConsole.Attributes;
using InteractiveConsole.Storage;

namespace InteractiveConsole.Commands
{
    [Command]
    public class PrintVariablesCommand : BaseCommand//, ICommand
    {
        private readonly IInMemoryStorage _inMemoryStorage;
        public PrintVariablesCommand(IInMemoryStorage inMemoryStorage)
        {
            _inMemoryStorage = inMemoryStorage;
        }

        public override object Execute()
        {
            Printer.PrintHeader($"Storage contains {_inMemoryStorage.Variables.Count} variables.");

            foreach (var variable in _inMemoryStorage.Variables)
            {
                Printer.Print($"{variable.Id}: ");
            }

            Printer.Print();

            return null;
        }
    }
}