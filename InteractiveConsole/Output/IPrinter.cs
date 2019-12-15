using System.Collections.Generic;
using InteractiveConsole.Models;

namespace InteractiveConsole.Output
{
    public interface IPrinter
    {
        Printer Write();
        Printer WriteLine();
        void Info(string text);
        void Info2(string text);
        void Success(string text);
        void Progress(string text);
        void Error(string text);
        void Highlight(string text);
        void None(string text);
        void Ascii(string text);
        void Print(List<CommandInfo> commands);
        void NewLine();
    }
}