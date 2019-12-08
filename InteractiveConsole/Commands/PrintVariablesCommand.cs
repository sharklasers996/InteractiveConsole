using System.Linq;
using System;
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
                var typeString = string.Empty;
                if (variable.IsList)
                {
                    if (variable.IsListItemCustomObject)
                    {
                        typeString = $"list of {variable.Length} {variable.ListItemObjectName} objects";
                    }
                    else
                    {
                        typeString = variable switch
                        {
                            var o when o.IsListItemNumber => $"list of {variable.Length} numbers",
                            var o when o.IsListItemString => $"list of {variable.Length} strings",
                            _ => $"list of {variable.Length} custom objects"
                        };
                    }
                }
                else
                {
                    if (variable.IsCustomObject)
                    {
                        typeString = $"{variable.ObjectName} object";
                    }
                    else
                    {
                        typeString = variable switch
                        {
                            var o when o.IsList => "list",
                            var o when o.IsNumber => "number",
                            var o when o.IsString => "string",
                            _ => "object"
                        };
                    }
                }

                Printer.WriteLine().Info2($"{typeString} returned by {variable.ProducedByCommand}");
            }

            Printer.WriteLine().Info(new string('-', 50));
            Printer.NewLine();

            return null;
        }
    }
}