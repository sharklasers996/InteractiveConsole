using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System;
using InteractiveConsole.Attributes;
using InteractiveConsole.Models;
using InteractiveConsole.Constants;
using System.Collections.Generic;

namespace InteractiveConsole.Commands
{
    [Command(Description = "Prints this information", Category = CommandCategories.BuiltIn)]
    public class PrintCommandsCommand : BaseCommand
    {
        private readonly ICommandDiscovery _commandDiscovery;

        public PrintCommandsCommand(ICommandDiscovery commandDiscovery)
        {
            _commandDiscovery = commandDiscovery;
        }

        public override object Execute()
        {
            var categories = _commandDiscovery
                .AvailableCommands
                .ToLookup(x => x.Category, y => y);

            Printer.WriteLine().Info("Available commands");
            var index = 0;
            foreach (var category in categories.OrderBy(x => x.Key))
            {
                var categoryName = category.Key;
                if (String.IsNullOrEmpty(categoryName))
                {
                    categoryName = "Uncategorized";
                }
                Printer.WriteLine().Highlight($"{categoryName}");

                foreach (var command in category.OrderBy(x => x.Name))
                {
                    Printer.Write().Highlight($"#{index} ");
                    Printer.Write().Info($"{command.NameWithoutSuffix} ");
                    foreach (var option in command.Options)
                    {
                        var requiredString = option.Required ? "required " : string.Empty;

                        Printer.Write().Highlight(option.Name);
                        Printer.Write().Info2($" ({requiredString}{option.TypeInfo.ToString()}) ");
                    }
                    Printer.NewLine();
                    index++;
                }
            }

            string selection;
            do
            {
                selection = Reader.LetterSelection(
                    new Dictionary<string, string> {
                    { "s", "Select command for more info" },
                    { "q", "Quit to console" }
                });

                if (selection == "s")
                {
                    var orderedCommands = _commandDiscovery
                        .AvailableCommands
                        .OrderBy(x => x.Category)
                        .ThenBy(x => x.Name)
                        .ToList();
                    var commandIndices = Reader.NumberSelection("Enter command numbers: ", 0, index);
                    foreach (var i in commandIndices.OrderBy(x => x))
                    {
                        var command = orderedCommands[i];

                        Printer.Write().Highlight($"#{i} ");
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
                        Printer.WriteLine().None(command.Description);
                    }
                }
            }
            while (selection != "q");

            return null;
        }
    }
}