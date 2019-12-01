using System;

namespace InteractiveConsole.Helpers
{
    public class PersistConsolePosition : IDisposable
    {
        private readonly int _cursorPosition;

        public PersistConsolePosition()
        {
            _cursorPosition = Console.CursorLeft;
        }

        public void Dispose()
        {
            Console.SetCursorPosition(_cursorPosition, Console.CursorTop);
        }
    }
}