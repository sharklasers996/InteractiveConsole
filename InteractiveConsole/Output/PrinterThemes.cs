using System.Drawing;

namespace InteractiveConsole.Output
{
    public class PrinterThemes
    {
        public PrinterTheme Default = new PrinterTheme
        {
            InfoPrimary = Color.FromArgb(0, 168, 222),
            InfoSecondary = Color.FromArgb(2, 121, 190),
            Error = Color.FromArgb(240, 78, 42),
            Progress = Color.FromArgb(255, 194, 14),
            Success = Color.FromArgb(47, 170, 79),
            Highlight = Color.FromArgb(206, 74, 152)
        };
    }
}