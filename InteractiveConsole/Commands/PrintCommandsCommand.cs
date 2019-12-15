using System;
using InteractiveConsole.Attributes;

namespace InteractiveConsole.Commands
{
    [Command(Description="Prints this information")]
    public class PrintCommandsCommand : BaseCommand
    {
        private readonly ICommandDiscovery _commandDiscovery;

        public PrintCommandsCommand(ICommandDiscovery commandDiscovery)
        {
            _commandDiscovery = commandDiscovery;
        }

        public override object Execute()
        {
            Printer.WriteLine().Info("Available commands");
            Printer.WriteLine().Info(new string('_', 50));

            foreach (var command in _commandDiscovery.AvailableCommands)
            {
                Printer.Write().Info($"{command.NameWithoutSuffix} ");
                foreach (var option in command.Options)
                {
                    var requiredString = option.Required ? "required " : string.Empty;

                    Printer.Write().Highlight(option.Name);
                    Printer.Write().Info2($" ({requiredString}{option.TypeInfo.ToString()}) ");
                }

                Printer.NewLine();

                if (!String.IsNullOrEmpty(command.Description))
                {
                    Printer.WriteLine().None($"{command.Description}");
                }
                Printer.WriteLine().Info(new string('_', 50));
            }
            Printer.NewLine();

            return null;
        }
    }
}