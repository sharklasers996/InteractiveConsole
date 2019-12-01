using System.Collections.Generic;
using InteractiveConsole.Models;

namespace InteractiveConsole.Output
{
    public interface IPrinter
    {
        void PrintHeader(string text);
        void PrintSubheader(string text);
        void Print(string text = null);
        void PrintAscii(string text, string font);
        void PrintCommands(List<CommandInfo> commands);
        string PrintWithSelection(object item, Dictionary<string, string> availableActions);
    }
}