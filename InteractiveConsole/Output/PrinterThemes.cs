using System.Drawing;

namespace InteractiveConsole.Output
{
    public class PrinterThemes
    {
        public static PrinterTheme Default = new PrinterTheme
        {
            Primary = Color.White,
            PrimaryVariant = Color.WhiteSmoke,
            Secondary = Color.White,
            SecondaryVariant = Color.WhiteSmoke,
            Highlight = Color.Wheat,
            Normal = Color.White
        };

        public static PrinterTheme LadiesNight = new PrinterTheme
        {
            Primary = Color.FromArgb(98, 0, 238),
            PrimaryVariant = Color.FromArgb(55, 0, 179),
            Secondary = Color.FromArgb(4, 218, 198),
            SecondaryVariant = Color.FromArgb(1, 135, 134),
            Highlight = Color.FromArgb(198, 4, 218),
            Normal = Color.White
        };
    }
}