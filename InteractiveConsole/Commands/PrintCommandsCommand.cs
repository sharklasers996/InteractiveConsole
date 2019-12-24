using System.Linq;
using System;
using InteractiveConsole.Attributes;
using InteractiveConsole.Models;
using InteractiveConsole.Constants;

namespace InteractiveConsole.Commands
{
    [Command(Description = "Prints this information", Category = CommandCategories.BuiltIn)]
    public class PrintCommandsCommand : BaseCommand
    {
        private readonly ICommandDiscovery _commandDiscovery;

        [CommandParameter]
        public string Category { get; set; }

        public PrintCommandsCommand(ICommandDiscovery commandDiscovery)
        {
            _commandDiscovery = commandDiscovery;
        }

        public override object Execute()
        {
            if (String.IsNullOrEmpty(Category))
            {
                Printer.WriteLine().Highlight("All commands");
            }
            else
            {
                Printer.WriteLine().Highlight($"{Category} commands");
            }
            Printer.WriteLine().Info(new string('_', 50));

            foreach (var command in _commandDiscovery.AvailableCommands.Where(CommandInCategory))
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

                if (String.IsNullOrEmpty(Category))
                {
                    var catString = $"{command.Category} category";
                    if (String.IsNullOrEmpty(command.Category))
                    {
                        catString = "Uncategorized";
                    }
                    Printer.WriteLine().Info2(catString);
                }

                Printer.WriteLine().Info(new string('_', 50));
            }

            Printer.NewLine();

            return null;
        }

        private bool CommandInCategory(CommandInfo commandInfo)
        {
            if (String.IsNullOrEmpty(Category))
            {
                return true;
            }

            if (String.IsNullOrEmpty(commandInfo.Category)
                && String.IsNullOrEmpty(Category))
            {
                return true;
            }

            if (!String.IsNullOrEmpty(commandInfo.Category)
                && commandInfo.Category.Equals(Category))
            {
                return true;
            }

            return false;
        }
    }
}