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
                Printer.WriteLine().Error("Could not find command");
                return null;
            }

            if (!command.Options.Any())
            {
                Printer.WriteLine().Error("Command doesn't have any options");
                return null;
            }

            Printer.WriteLine().Info(command.NameWithoutSuffix);
            foreach (var option in command.Options)
            {
                Printer.Write().Info($"\t{option.Name} ");
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

                Printer.WriteLine().Info2($"({requiredString}{typeString})");
                if (!option.AvailableValues.Any())
                {
                    continue;
                }

                foreach (var value in option.AvailableValues)
                {
                    Printer.WriteLine().Info2($"\t\t{value}");
                }
            }

            Printer.NewLine();
            Printer.WriteLine().Info(command.Description);

            return null;
        }
    }
}