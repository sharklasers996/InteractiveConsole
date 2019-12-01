using System;

namespace InteractiveConsole.Extensions
{
    public static class InputHandlerExtensions
    {
        public static string ToKeyString(this ConsoleKeyInfo keyInfo)
        {
            return (keyInfo.Modifiers != ConsoleModifiers.Control && keyInfo.Modifiers != ConsoleModifiers.Shift) ?
                keyInfo.Key.ToString() : keyInfo.Modifiers.ToString() + keyInfo.Key.ToString();
        }
    }
}