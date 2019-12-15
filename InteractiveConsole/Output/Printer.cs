using System;
using Pastel;
using Figgle;

namespace InteractiveConsole.Output
{
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
    }
}