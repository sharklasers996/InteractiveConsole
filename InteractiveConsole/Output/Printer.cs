using System.Threading;
using System.Linq;
using System.Collections.Generic;
using Colorful;
using InteractiveConsole.Models;
using Console = Colorful.Console;

namespace InteractiveConsole.Output
{
    public class Printer : IPrinter
    {
        private readonly PrinterTheme _theme;
        private static SynchronizationContext _ctx;
        private static Thread _current;

        public Printer(PrinterTheme theme)
        {
            _theme = theme;
            _ctx = SynchronizationContext.Current;
            _current = Thread.CurrentThread;
        }

        public void PrintAscii(string text, string font)
        {
            var figletFont = FigletFont.Parse(font);
            var figlet = new Figlet(figletFont);
            Console.WriteLine(figlet.ToAscii(text), _theme.Primary);
        }

        public void PrintHeader(string text)
        {
            Console.WriteLine(text, _theme.Primary);
        }

        public void PrintSubheader(string text)
        {
            Console.WriteLine(text, _theme.Primary);
        }

        public void Print(string text = null)
        {
            Console.WriteLine(text);
        }

        public void Print(object obj)
        {
            Console.WriteLine(obj);
        }

        public void PrintCommands(List<CommandInfo> commands)
        {
            Console.WriteLine("Available commands:", _theme.Secondary);
            Console.WriteLine();

            foreach (var command in commands)
            {
                Console.WriteLine($"\t{command.NameWithoutSuffix}", _theme.SecondaryVariant);
                foreach (var option in command.Options)
                {
                    Console.WriteLine($"\t\t> {option.Name}", _theme.Normal);
                }
            }
            Console.WriteLine();
        }

        public string PrintWithSelection(object item, Dictionary<string, string> availableActions)
        {
            Console.WriteLine(item.ToString());

            var selectionString = string.Empty;
            foreach (var keyValue in availableActions)
            {
                selectionString += $"({keyValue.Key}) {keyValue.Value} | ";
            }

            selectionString = selectionString.Trim(new[] { ' ', '|' });
            Console.WriteLine(selectionString, _theme.Highlight);

            var keyInfo = Console.ReadKey(true);
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(" ");
            Console.SetCursorPosition(0, Console.CursorTop);
            while (!availableActions.TryGetValue(keyInfo.KeyChar.ToString(), out _))
            {
                Console.WriteLine($"Selection '{keyInfo.KeyChar}' is not available");
                keyInfo = Console.ReadKey(true);
            }

            return keyInfo.KeyChar.ToString();
        }
    }
}