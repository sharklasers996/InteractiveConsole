using System;
using InteractiveConsole;
using InteractiveConsole.Output;

namespace WebCrawler
{
    class Program
    {
        static void Main() => new  InteractiveConsoleBuilder()
            .WithAsciiTitle("Web Crawler")
            .WithTheme(PrinterThemes.Default)
            .WithWelcomeText("Download a web page and query it using xpath.")
            .Run();
    }
}
