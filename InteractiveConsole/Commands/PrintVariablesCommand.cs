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
            Console.WriteLine($"Storage contains {_inMemoryStorage.Variables.Count} variables.");
            Console.WriteLine(new string('-', 50));

            foreach (var variable in _inMemoryStorage.Variables)
            {
                Console.Write($"{variable.Id}: ");
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

                Console.WriteLine($"{typeString} returned by {variable.ProducedByCommand}");
            }

            Console.WriteLine(new string('-', 50));
            Console.WriteLine();

            return null;
        }
    }
}