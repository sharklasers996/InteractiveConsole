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

                Printer.WriteLine().Info2($"({requiredString}{option.TypeInfo.ToString()})");
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