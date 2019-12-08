using System.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using InteractiveConsole.Extensions;
using InteractiveConsole.Helpers;
using InteractiveConsole.Output;

namespace InteractiveConsole
{
    public class InputHandler : IInputHandler
    {
        private readonly Dictionary<string, Action> _keyActions;
        private readonly List<string> _history = new List<string>();
        private int _historyIndex;

        private readonly StringBuilder _text = new StringBuilder();
        private int _cursorPosition;
        private int _cursorLimitRight;
        private int _cursorLimitLeft;

        private bool IsStartOfLine { get { return _cursorPosition == 0 || _cursorPosition <= _cursorLimitLeft; } }
        private bool IsEndOfLine { get { return _cursorPosition == _cursorLimitRight; } }

        private readonly IAutoCompleteHandler _autoComplete;
        private readonly PrinterTheme _printerTheme;

        public InputHandler(IAutoCompleteHandler autoCompleteHandler, PrinterTheme printerTheme)
        {
            _printerTheme = printerTheme;
            _autoComplete = autoCompleteHandler;

            _keyActions = new Dictionary<string, Action>
            {
                ["ControlA"] = MoveCursorHome,
                ["ControlB"] = MoveCursorLeft,
                ["ControlF"] = MoveCursorRight,
                ["ControlE"] = MoveCursorEnd,
                ["Backspace"] = DeleteLeft,
                ["ControlD"] = DeleteRight,
                ["Tab"] = () => Complete(next: true),
                ["ShiftTab"] = () => Complete(next: false),
                ["ControlN"] = () => CompleteSelection(next: true),
                ["ControlP"] = () => CompleteSelection(next: false),
                ["ControlBackspace"] = DeleteWordLeft,
                ["AltD"] = DeleteWordRight,
                ["UpArrow"] = PreviousCommandInHistory,
                ["DownArrow"] = NextCommandInHistory
            };
        }

        public string ReadLine(bool masked = false)
        {
            var keyInfo = Console.ReadKey(true);
            while (keyInfo.Key != ConsoleKey.Enter)
            {
                if (_keyActions.TryGetValue(keyInfo.ToKeyString(), out var action))
                {
                    action.Invoke();
                }
                else
                {
                    if (masked)
                    {
                        WriteCharMasked(keyInfo.KeyChar);
                    }
                    else
                    {
                        WriteChar(keyInfo.KeyChar);
                    }
                }

                keyInfo = Console.ReadKey(true);
            }

            Console.WriteLine();

            var result = _text.ToString();

            _text.Clear();
            _cursorPosition = 0;
            _cursorLimitRight = 0;
            _cursorLimitLeft = 0;

            _history.Add(result);
            _historyIndex = 0;

            return result;
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

        private void Complete(bool next)
        {
            var result = _autoComplete.Complete(_text.ToString(), next);
            _text.Clear();
            _text.Append(result);

            _cursorPosition = _text.Length;

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(result + new string(' ', _cursorLimitRight));
            Console.SetCursorPosition(_text.Length, Console.CursorTop);

            _cursorLimitRight = _text.Length;
            //  ApplyColors();
        }

        private void CompleteSelection(bool next)
        {
            string result = _autoComplete.CompleteOptionSelection(_text.ToString(), _cursorPosition, next);
            var lengthDifference = result.Length - _text.Length;

            _text.Clear();
            _text.Append(result);

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(result + new string(' ', _cursorLimitRight));

            if (_text.Length < _cursorPosition)
            {
                _cursorPosition = _text.Length;
            }
            _cursorPosition += lengthDifference;
            Console.SetCursorPosition(_cursorPosition, Console.CursorTop);

            _cursorLimitRight = _text.Length;
            MoveCursorRightUntilFirstSpaceOrEnd();
            //  ApplyColors();
        }

        private void ApplyColors()
        {
            var text = _text.ToString();
            var commandMatch = Regex.Match(text, @"(^\w+\s|\w+)");

            var parameters = new List<Parameter>();
            var parameterMatches = Regex.Matches(text, "(\\s(?<parameterName>\\w+)=((?<parameterValue>\\w+)|\"(?<parameterValue>([^\"\\\\]|\\\\.)*)\")|\\s(?<parameterName>\\w+)=)");
            foreach (Match match in parameterMatches)
            {
                parameters.Add(new Parameter
                {
                    Name = match.Groups["parameterName"].ToString(),
                    Value = match.Groups["parameterValue"].ToString()
                });
            }

            if (commandMatch.Success)
            {
                var wrote = 0;
                using (new PersistConsolePosition())
                {
                    System.Console.SetCursorPosition(0, System.Console.CursorTop);
                    System.Console.Write($"{commandMatch.Value}", Color.AliceBlue);
                    wrote += commandMatch.Value.Length;
                    foreach (var param in parameters)
                    {
                        wrote += $"{param.Name}=".Length;
                        wrote += $"{param.Value}".Length;
                        System.Console.Write($"{param.Name}=", Color.Azure);
                        System.Console.Write($"{param.Value}");
                    }
                }
            }
        }

        private void WriteCharMasked(char input)
        {
            if (IsEndOfLine)
            {
                _text.Append(input);
                Console.Write('*');
                _cursorPosition++;
            }
            else
            {
                var textAfterCursor = _text.ToString().Substring(_cursorPosition);
                _text.Insert(_cursorPosition, input);

                using (new PersistConsolePosition())
                {
                    Console.Write(new string('*', textAfterCursor.Length + 1));
                }

                MoveCursorRight();
            }

            _cursorLimitRight++;
        }

        private void WriteChar(char input)
        {
            if (IsEndOfLine)
            {
                _text.Append(input);
                Console.Write(input.ToString());
                _cursorPosition++;
            }
            else
            {
                var textAfterCursor = _text.ToString().Substring(_cursorPosition);
                _text.Insert(_cursorPosition, input);

                using (new PersistConsolePosition())
                {
                    Console.Write(input.ToString() + textAfterCursor);
                }

                MoveCursorRight();
            }

            _cursorLimitRight++;
        }

        private void DeleteLeft()
        {
            if (IsStartOfLine)
            {
                return;
            }

            MoveCursorLeft();

            _text.Remove(_cursorPosition, 1);
            var replacement = _text.ToString(_cursorPosition, _text.Length - _cursorPosition);

            using (new PersistConsolePosition())
            {
                Console.Write($"{replacement} ");
            }

            _cursorLimitRight--;
        }

        private void DeleteRight()
        {
            if (IsEndOfLine)
            {
                return;
            }

            MoveCursorRight();
            DeleteLeft();
        }

        private void DeleteWordLeft()
        {
            var textBeforeCursor = _text.ToString(0, _cursorPosition);
            var lastSpaceIndex = textBeforeCursor.LastIndexOf(' ');
            if (lastSpaceIndex == -1)
            {
                lastSpaceIndex = _cursorLimitLeft;
            }

            var wordLength = _cursorPosition - lastSpaceIndex;
            for (var i = 0; i < wordLength; i++)
            {
                DeleteLeft();
            }
        }

        private void DeleteWordRight()
        {
            var textAfterCursor = _text
                .ToString(_cursorPosition, _text.Length - _cursorPosition)
                .Trim();
            var wordLength = textAfterCursor.IndexOf(' ');
            if (wordLength == -1)
            {
                wordLength = _cursorLimitRight;
            }

            for (var i = 0; i < wordLength; i++)
            {
                DeleteRight();
            }
        }

        private void PreviousCommandInHistory()
        {
            _historyIndex--;
            if (_historyIndex < 0)
            {
                _historyIndex = _history.Count - 1;
            }
            PrintHistoryCommand();
        }

        private void NextCommandInHistory()
        {
            _historyIndex++;
            if (_historyIndex >= _history.Count)
            {
                _historyIndex = 0;
            }
            PrintHistoryCommand();
        }

        private void PrintHistoryCommand()
        {

            _text.Clear();
            _text.Append(_history[_historyIndex]);

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(_history[_historyIndex] + new string(' ', _cursorLimitRight));
            Console.SetCursorPosition(_text.Length, Console.CursorTop);

            _cursorPosition = _text.Length;
            _cursorLimitRight = _text.Length;
        }

        private void MoveCursorHome()
        {
            Console.SetCursorPosition(_cursorLimitLeft, Console.CursorTop);
            _cursorPosition = _cursorLimitLeft;
        }

        private void MoveCursorEnd()
        {
            Console.SetCursorPosition(_cursorLimitRight, Console.CursorTop);
            _cursorPosition = _cursorLimitRight;
        }

        private void MoveCursorLeft()
        {
            if (IsStartOfLine)
            {
                return;
            }

            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);

            _cursorPosition--;
        }

        private void MoveCursorRight()
        {
            if (IsEndOfLine)
            {
                return;
            }

            Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);

            _cursorPosition++;
        }

        private void MoveCursorRightUntilFirstSpaceOrEnd()
        {
            if (_cursorPosition >= _text.Length - 1)
            {
                return;
            }

            var charAtCursor = _text[_cursorPosition];
            if (charAtCursor == ' ')
            {
                return;
            }

            var textBeforeCursor = _text.ToString(0, _cursorPosition);
            var textAfterCursor = _text.ToString(_cursorPosition, _text.Length - _cursorPosition);
            var nextSpaceIndex = textAfterCursor.IndexOf(" ");
            if (nextSpaceIndex == -1)
            {
                MoveCursorEnd();
                return;
            }

            _cursorPosition = textBeforeCursor.Length + nextSpaceIndex;
            Console.SetCursorPosition(_cursorPosition, Console.CursorTop);
        }
    }

}