using System;
using System.Net;
using InteractiveConsole.Attributes;
using InteractiveConsole;
using HtmlAgilityPack;

namespace WebCrawler
{
    [Command(Description = "Downloads a web page and saves it to in-memory storage.")]
    public class DownloadWebPageCommand : BaseCommand
    {
        [CommandParameter]
        [Required]
        public string Url { get; set; }

        public override bool IsValid()
        {
            if (Url.IndexOf("http://", StringComparison.InvariantCultureIgnoreCase) == -1
                || Url.IndexOf("https://", StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                Url = "https://" + Url;
            }

            try
            {
                new Uri(Url);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public override object Execute()
        {
            Printer.WriteLine().Progress($"Downloading {Url}");

            using var webClient = new WebClient();
            var source = webClient.DownloadString(Url);

            Printer.WriteLine().Success("Done!");

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(source);

            return htmlDocument;
        }
    }
}