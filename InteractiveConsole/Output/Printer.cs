using System.Linq;
using System;
using System.Collections.Generic;
using InteractiveConsole.Models;
using Pastel;
using Figgle;

namespace InteractiveConsole.Output
{
    // TODO: Clean up
    public class Printer : IPrinter
    {
        private readonly PrinterTheme _theme;

        private bool _writeLine = false;

        public Printer(PrinterTheme theme)
        {
            _theme = theme;
        }

        public Printer Write()
        {
            _writeLine = false;
            return this;
        }

        public Printer WriteLine()
        {
            _writeLine = true;
            return this;
        }

        private Action<string> ConsoleWrite
        {
            get
            {
                if (_writeLine)
                {
                    return Console.WriteLine;
                }

                return Console.Write;
            }
        }

        public void Ascii(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return;
            }

            ConsoleWrite.Invoke(FiggleFonts.Standard.Render(text).Pastel(_theme.Highlight));
        }

        public void Error(string text)
        {
            ConsoleWrite.Invoke(text?.Pastel(_theme.Error));
        }

        public void Highlight(string text)
        {
            ConsoleWrite.Invoke(text?.Pastel(_theme.Highlight));
        }

        public void Info(string text)
        {
            ConsoleWrite.Invoke(text?.Pastel(_theme.InfoPrimary));
        }

        public void Info2(string text)
        {
            ConsoleWrite.Invoke(text?.Pastel(_theme.InfoSecondary));
        }

        public void Success(string text)
        {
            ConsoleWrite.Invoke(text?.Pastel(_theme.Success));
        }

        public void Progress(string text)
        {
            ConsoleWrite.Invoke(text?.Pastel(_theme.Progress));
        }

        public void None(string text)
        {
            ConsoleWrite.Invoke(text);
        }

        public void NewLine()
        {
            Console.WriteLine();
        }

        public void Print(List<CommandInfo> commands)
        {
            Console.WriteLine("Available commands".Pastel(_theme.InfoPrimary));
            Console.WriteLine(new string('_', 50).Pastel(_theme.InfoPrimary));

            foreach (var command in commands)
            {
                Console.Write($"{command.NameWithoutSuffix} ".Pastel(_theme.InfoPrimary));
                var optionsString = string.Empty;
                foreach (var option in command.Options)
                {
                    var requiredString = option.Required ? "required " : string.Empty;

                    optionsString += option.Name.Pastel(_theme.Highlight);
                    optionsString += $" ({requiredString}{option.TypeInfo.ToString()}) ".Pastel(_theme.InfoSecondary);
                }

                Console.Write(optionsString.Trim(new[] { ' ', ',' }));
                Console.WriteLine();

                if (!String.IsNullOrEmpty(command.Description))
                {
                    Console.WriteLine($"{command.Description}");
                }
                Console.WriteLine(new string('_', 50).Pastel(_theme.InfoPrimary));
            }
            Console.WriteLine();
        }

        public string Selection(object item, Dictionary<string, string> availableActions)
        {
            Console.WriteLine(item?.ToString());

            var actionsTextLength = 0;
            for (var i = 0; i < availableActions.Count; i++)
            {
                var actionKeyString = $"({availableActions.ElementAt(i).Key}) ";
                var actionValueString = $"{availableActions.ElementAt(i).Value} ";
                Console.Write(actionKeyString.Pastel(_theme.Highlight));
                Console.Write(actionValueString);

                actionsTextLength += actionKeyString.Length;
                actionsTextLength += actionValueString.Length;
            }
            Console.WriteLine();

            var keyInfo = Console.ReadKey(true);
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(" ");
            Console.SetCursorPosition(0, Console.CursorTop);
            while (!availableActions.TryGetValue(keyInfo.KeyChar.ToString(), out _))
            {
                Console.WriteLine($"Selection '{keyInfo.KeyChar}' is not available".Pastel(_theme.Error));
                keyInfo = Console.ReadKey(true);
            }

            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', actionsTextLength));
            Console.SetCursorPosition(0, Console.CursorTop - 1);

            return keyInfo.KeyChar.ToString();
        }
    }
}