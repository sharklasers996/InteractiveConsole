using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InteractiveConsole.Attributes;
using InteractiveConsole.Extensions;

namespace InteractiveConsole.Commands
{
    [Command]
    public class PrintCommandInfoCommand : BaseCommand//, ICommand
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
                Printer.Print("Could not find command");
                return null;
            }

            if (!command.Options.Any())
            {
                Printer.Print("Command doesn't have any options");
                return null;
            }

            Printer.Print($"Command {command.NameWithoutSuffix} has {command.Options.Count} options");
            foreach (var option in command.Options)
            {

                Printer.Print($"Option {option.Name}, required = {option.Required}");
                if (option.Type.IsGenericType
                                && option.Type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Printer.Print($"Option {option.Name} is a list");
                }

                if (!option.AvailableValues.Any())
                {
                    continue;
                }

                Printer.Print($"Option {option.Name} has available values: {string.Join(',', option.AvailableValues)}");
            }

            return null;
        }
    }
}