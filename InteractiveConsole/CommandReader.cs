using System;

namespace InteractiveConsole
{
    public class CommandReader : BaseInputHandler, ICommandReader
    {
        private readonly IAutoCompleteHandler _autoComplete;

        public CommandReader(IAutoCompleteHandler autoCompleteHandler)
        {
            _autoComplete = autoCompleteHandler;

            _keyActions.Add("Tab", () => Complete(next: true));
            _keyActions.Add("ShiftTab", () => Complete(next: false));
            _keyActions.Add("ControlN", () => CompleteSelection(next: true));
            _keyActions.Add("ControlP", () => CompleteSelection(next: false));
        }

        public string ReadLine()
        {
            return ReadLine(masked: false);
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
    }
}