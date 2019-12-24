using System.Drawing;

namespace InteractiveConsole.Output
{
    public static class PrinterThemes
    {
        public static PrinterTheme Default = new PrinterTheme
        {
            InfoPrimary = Color.FromArgb(0, 168, 222),
            InfoSecondary = Color.FromArgb(2, 121, 190),
            Error = Color.FromArgb(240, 78, 42),
            Progress = Color.FromArgb(255, 194, 14),
            Success = Color.FromArgb(47, 170, 79),
            Highlight = Color.FromArgb(206, 74, 152)
        };

        public static PrinterTheme Green = new PrinterTheme
        {
            InfoPrimary = Color.FromArgb(47, 170, 79),
            InfoSecondary = Color.FromArgb(9, 122, 63),
            Error = Color.FromArgb(240, 78, 42),
            Progress = Color.FromArgb(165, 207, 78),
            Success = Color.FromArgb(0, 168, 222),
            Highlight = Color.FromArgb(255, 194, 14)
        };
        
        public static PrinterTheme Neon = new PrinterTheme
        {
            InfoPrimary = Color.FromArgb(255, 121, 198),
            InfoSecondary = Color.FromArgb(57, 86, 153),
            Error = Color.FromArgb(177, 62, 52),
            Progress = Color.FromArgb(247, 140, 93),
            Success = Color.FromArgb(105, 177, 121),
            Highlight = Color.FromArgb(80, 250, 123)
        };
    }
}