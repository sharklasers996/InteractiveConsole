using System;
using System.Collections.Generic;
using System.Linq;
using InteractiveConsole.Attributes;

namespace InteractiveConsole.Commands
{
    [Command(description: "Prints information about command and its options")]
    public class PrintCommandInfoCommand : BaseCommand
    {
        [CommandParameter]
        public string CommandName { get; set; }

        private readonly ICommandDiscovery _commandDiscovery;

        public PrintCommandInfoCommand(ICommandDiscovery commandDiscovery)
        {
            _commandDiscovery = commandDiscovery;

        }
        public override object Execute()
        {
            var command = _commandDiscovery.AvailableCommands.FirstOrDefault(x => x.NameWithoutSuffix == CommandName);
            if (command == null)
            {
                Console.WriteLine("Could not find command");
                return null;
            }

            if (!command.Options.Any())
            {
                Console.WriteLine("Command doesn't have any options");
                return null;
            }

            Console.WriteLine(command.NameWithoutSuffix);
            foreach (var option in command.Options)
            {
                Console.Write($"\t{option.Name} ");
                var requiredString = option.Required ? "required " : string.Empty;

                var typeString = string.Empty;
                if (option.IsList)
                {
                    if (option.IsListItemCustomObject)
                    {
                        typeString = $"list of {option.ListItemObjectName} objects";
                    }
                    else
                    {
                        typeString = option switch
                        {
                            var o when o.IsListItemEnum => "list of enums",
                            var o when o.IsListItemNumber => "list of numbers",
                            var o when o.IsListItemString => "list of strings",
                            _ => "list of custom objects"
                        };
                    }
                }
                else
                {
                    if (option.IsCustomObject)
                    {
                        typeString = $"{option.ObjectName} object";
                    }
                    else
                    {
                        typeString = option switch
                        {
                            var o when o.IsEnum => "enum",
                            var o when o.IsList => "list",
                            var o when o.IsNumber => "number",
                            var o when o.IsString => "string",
                            _ => "object"
                        };
                    }
                }

                Console.WriteLine($"({requiredString}{typeString})");
                if (!option.AvailableValues.Any())
                {
                    continue;
                }

                foreach (var value in option.AvailableValues)
                {
                    Console.WriteLine($"\t\t{value}");
                }
            }

            Console.WriteLine();
            Console.WriteLine(command.Description);

            return null;
        }
    }
}