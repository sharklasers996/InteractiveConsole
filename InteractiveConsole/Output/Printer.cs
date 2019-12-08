using System;
using System.Threading;
using System.Collections.Generic;
using Colorful;
using InteractiveConsole.Models;
using Console = Colorful.Console;

namespace InteractiveConsole.Output
{
    public class Printer : IPrinter
    {
        private readonly PrinterTheme _theme;

        public Printer(PrinterTheme theme)
        {
            _theme = theme;
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
            Console.WriteLine("Available commands");
            Console.WriteLine(new string('-', 50));

            foreach (var command in commands)
            {
                Console.Write($"{command.NameWithoutSuffix} ");
                var optionsString = string.Empty;
                foreach (var option in command.Options)
                {
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

                    optionsString += $"{option.Name} ({requiredString}{typeString}), ";
                }

                Console.Write(optionsString.Trim(new[] { ' ', ',' }));
                Console.WriteLine();

                if (!String.IsNullOrEmpty(command.Description))
                {
                    Console.WriteLine($"\t{command.Description}");
                }
            }
            Console.WriteLine(new string('-', 50));
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