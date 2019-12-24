using System;
using System.Linq;
using InteractiveConsole.Attributes;
using InteractiveConsole.Constants;

namespace InteractiveConsole.Commands
{
    [Command(Description = "Prints available command categories", Category = CommandCategories.BuiltIn)]
    public class PrintCommandCategoriesCommand : BaseCommand
    {
        private readonly ICommandDiscovery _commandDiscovery;

        public PrintCommandCategoriesCommand(ICommandDiscovery commandDiscovery)
        {
            _commandDiscovery = commandDiscovery;

        }

        public override object Execute()
        {
            var categories = _commandDiscovery
                .AvailableCommands
                .Select(x => x.Category)
                .Distinct();

            Printer.WriteLine().Highlight("Available categories");
            Printer.WriteLine().Info(new string('_', 50));

            foreach (var cat in categories)
            {
                var catString = cat;
                if (String.IsNullOrEmpty(catString))
                {
                    catString = "Uncategorized";
                }
                var categoryCommandCount = _commandDiscovery.AvailableCommands.Count(x => x.Category == cat);
                
                Printer.Write().Info(catString);
                Printer.WriteLine().Highlight($" {categoryCommandCount} commands");
                Printer.WriteLine().Info(new string('_', 50));
            }

            return null;
        }
    }
}