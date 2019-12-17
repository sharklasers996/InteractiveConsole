using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using InteractiveConsole.Output;
using Pastel;

namespace InteractiveConsole
{
    public class Reader : BaseInputHandler, IReader
    {
        private readonly PrinterTheme _theme;

        public Reader(PrinterTheme theme)
        {
            _theme = theme;
        }

        public string Prompt(string prompt, bool masked = false)
        {
            _text.Clear();
            _text.Append(prompt);

            _cursorLimitLeft = _text.Length;
            _cursorLimitRight = _text.Length;
            _cursorPosition = _text.Length;

            Console.Write(_text.ToString());

            return ReadLine(masked).Replace(prompt, string.Empty);
        }

        public List<int> NumberSelection(string prompt)
        {
            _text.Clear();
            _text.Append(prompt);

            _cursorLimitLeft = _text.Length;
            _cursorLimitRight = _text.Length;
            _cursorPosition = _text.Length;

            Console.Write(_text.ToString());

            var numberStringList = ReadLine()
                .Replace(prompt, string.Empty)
                .Split(' ')
                .ToList();

            var numbersList = new List<int>();
            foreach (var numberString in numberStringList)
            {
                if (int.TryParse(numberString, out int number))
                {
                    numbersList.Add(number);
                    continue;
                }
                var rangeMatch = Regex.Match(numberString, @"\[(?<from>\d+)..(?<to>\d+)\]");
                if (rangeMatch.Success)
                {
                    var from = int.Parse(rangeMatch.Groups["from"].ToString());
                    var to = int.Parse(rangeMatch.Groups["to"].ToString());

                    for (var i = from; i <= to; i++)
                    {
                        numbersList.Add(i);
                    }
                }
            }

            return numbersList;
        }

        public string LetterSelection(object item, Dictionary<string, string> availableActions)
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

        public bool YesNoPrompt(string text)
        {
            var result = LetterSelection(text, new Dictionary<string, string> {
                { "y", "Yes" },
                { "n", "No"}
            });

            return result == "y";
        }
    }
}