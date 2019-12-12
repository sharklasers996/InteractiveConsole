using System.Linq;
using InteractiveConsole.Attributes;
using InteractiveConsole.Storage;

namespace InteractiveConsole.Commands
{
    [Command(description: "Prints a list of variables in memory")]
    public class PrintVariablesCommand : BaseCommand
    {
        private readonly IInMemoryStorage _inMemoryStorage;
        public PrintVariablesCommand(IInMemoryStorage inMemoryStorage)
        {
            _inMemoryStorage = inMemoryStorage;
        }

        public override object Execute()
        {
            if (!_inMemoryStorage.Variables.Any())
            {
                Printer.WriteLine().Info("Storage doesn't contain any variables.");
                return null;
            }

            Printer.WriteLine().Info($"Storage contains {_inMemoryStorage.Variables.Count} variables.");
            Printer.WriteLine().Info(new string('-', 50));

            foreach (var variable in _inMemoryStorage.Variables)
            {
                Printer.Write().Info($"{variable.Id}: ");
                Printer.WriteLine().Info2(variable.ToTypeString());
            }

            Printer.WriteLine().Info(new string('-', 50));
            Printer.NewLine();

            return null;
        }
    }
}