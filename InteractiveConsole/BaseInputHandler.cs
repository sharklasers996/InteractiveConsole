using System;
using System.Collections.Generic;
using System.Text;
using InteractiveConsole.Extensions;
using InteractiveConsole.Helpers;

namespace InteractiveConsole
{
    public abstract class BaseInputHandler
    {
        protected readonly Dictionary<string, Action> _keyActions;
        protected readonly List<string> _history = new List<string>();
        protected int _historyIndex;

        protected readonly StringBuilder _text = new StringBuilder();
        protected int _cursorPosition;
        protected int _cursorLimitRight;
        protected int _cursorLimitLeft;

        protected bool IsStartOfLine { get { return _cursorPosition == 0 || _cursorPosition <= _cursorLimitLeft; } }
        protected bool IsEndOfLine { get { return _cursorPosition == _cursorLimitRight; } }

        public BaseInputHandler()
        {
            _keyActions = new Dictionary<string, Action>
            {
                ["ControlA"] = MoveCursorHome,
                ["ControlB"] = MoveCursorLeft,
                ["ControlF"] = MoveCursorRight,
                ["ControlE"] = MoveCursorEnd,
                ["Backspace"] = DeleteLeft,
                ["ControlD"] = DeleteRight,
                ["ControlBackspace"] = DeleteWordLeft,
                ["AltD"] = DeleteWordRight,
                ["UpArrow"] = PreviousCommandInHistory,
                ["DownArrow"] = NextCommandInHistory
            };
        }

        protected string ReadLine(bool masked = false)
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


        protected void WriteCharMasked(char input)
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

        protected void WriteChar(char input)
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

        protected void DeleteLeft()
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

        protected void DeleteRight()
        {
            if (IsEndOfLine)
            {
                return;
            }

            MoveCursorRight();
            DeleteLeft();
        }

        protected void DeleteWordLeft()
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

        protected void DeleteWordRight()
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

        protected void PreviousCommandInHistory()
        {
            if (_history.Count == 0)
            {
                return;
            }

            _historyIndex--;
            if (_historyIndex < 0)
            {
                _historyIndex = _history.Count - 1;
            }
            PrintHistoryCommand();
        }

        protected void NextCommandInHistory()
        {
            if (_history.Count == 0)
            {
                return;
            }

            _historyIndex++;
            if (_historyIndex >= _history.Count)
            {
                _historyIndex = 0;
            }
            PrintHistoryCommand();
        }

        protected void PrintHistoryCommand()
        {

            _text.Clear();
            _text.Append(_history[_historyIndex]);

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(_history[_historyIndex] + new string(' ', _cursorLimitRight));
            Console.SetCursorPosition(_text.Length, Console.CursorTop);

            _cursorPosition = _text.Length;
            _cursorLimitRight = _text.Length;
        }

        protected void MoveCursorHome()
        {
            Console.SetCursorPosition(_cursorLimitLeft, Console.CursorTop);
            _cursorPosition = _cursorLimitLeft;
        }

        protected void MoveCursorEnd()
        {
            Console.SetCursorPosition(_cursorLimitRight, Console.CursorTop);
            _cursorPosition = _cursorLimitRight;
        }

        protected void MoveCursorLeft()
        {
            if (IsStartOfLine)
            {
                return;
            }

            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);

            _cursorPosition--;
        }

        protected void MoveCursorRight()
        {
            if (IsEndOfLine)
            {
                return;
            }

            Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);

            _cursorPosition++;
        }

        protected void MoveCursorRightUntilFirstSpaceOrEnd()
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