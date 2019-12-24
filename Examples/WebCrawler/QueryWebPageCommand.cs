using System;
using InteractiveConsole.Attributes;
using InteractiveConsole;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace WebCrawler
{
    [Command]
    public class QueryWebPageCommand : BaseCommand
    {
        [CommandParameter]
        [Required]
        public HtmlDocument HtmlDocument { get; set; }

        [CommandParameter]
        [Required]
        public string Xpath { get; set; }

        public override object Execute()
        {
            var nodes = HtmlDocument.DocumentNode.SelectNodes(Xpath);
            if (nodes == null)
            {
                Printer.WriteLine().Info("Could not find any nodes");
                return null;
            }
            Printer.WriteLine().Success($"Found {nodes.Count} nodes");

            for (var i = 0; i < nodes.Count; i++)
            {
                Printer.WriteLine().Info($"#{i}: {nodes[i].Name} - {nodes[i].Attributes.Count} attributes");
                var innerText = nodes[i].InnerText;
                if (!String.IsNullOrEmpty(innerText))
                {
                    if (innerText.Length > 300)
                    {
                        innerText = innerText.Substring(0, 300);
                        innerText += " [Truncated]";
                    }
                    Printer.WriteLine().Info2(innerText);
                }
                Printer.NewLine();
            }
            string selection;
            do
            {
                selection = Reader.LetterSelection(
                    new Dictionary<string, string> {
                    { "a", "Print each node attributes" },
                    { "s", "Select attribute to save" },
                    { "i", "Save inner texts" },
                    { "q", "Quit" }
                });

                if (selection == "i")
                {
                    return SaveInnerTexts(nodes);
                }

                if (selection == "a")
                {
                    PrintAttributes(nodes);
                }

                if (selection == "s")
                {
                    var attributes = SelectAttribute(nodes.ToList());

                    string attributesAction;
                    do
                    {
                        attributesAction = Reader.LetterSelection(
                            new Dictionary<string, string> {
                            { "s", "Save results" },
                            { "t", "Try different attribute" },
                            { "d", "Discard results" }
                        });

                        if (attributesAction == "s")
                        {
                            return attributes;
                        }

                        if (attributesAction == "t")
                        {
                            attributes = SelectAttribute(nodes.ToList());
                        }
                    }
                    while (attributesAction != "d");
                }
            }
            while (selection != "q");

            return null;
        }

        private List<string> SelectAttribute(List<HtmlNode> nodes)
        {
            var attributeName = Reader.Prompt("Attribute name: ");
            if (String.IsNullOrEmpty(attributeName))
            {
                Printer.WriteLine().Error("Attribute cannot be empty");
                return null;
            }

            var result = new List<string>();
            for (var i = 0; i < nodes.Count; i++)
            {
                var attribute = nodes[i].Attributes[attributeName];
                if (attribute != null)
                {
                    result.Add(attribute.Value);
                    Printer.WriteLine().Info($"#{i}: {attribute.Value}");
                }
                else
                {
                    Printer.WriteLine().Info($"#{i} doesn't have attribute '{attributeName}'");
                }
                Printer.NewLine();
            }

            Printer.WriteLine().Success($"Collected {result.Count} results.");
            return result;
        }

        private List<string> SaveInnerTexts(HtmlNodeCollection nodes)
        {
            if (Reader.YesNoPrompt("Cherry pick nodes?"))
            {
                var nodeNumbers = Reader.NumberSelection("Enter node numbers: ", 0, nodes.Count - 1);
                var result = new List<string>();
                foreach (var i in nodeNumbers)
                {
                    result.Add(nodes[i].InnerText.Trim());
                }

                return result;
            }

            return nodes.Select(x => x.InnerText).ToList();
        }

        private void PrintAttributes(HtmlNodeCollection nodes)
        {
            for (var i = 0; i < nodes.Count; i++)
            {
                Printer.WriteLine().Info($"#{i}: {nodes[i].Name} {nodes[i].Attributes.Count} attributes");
                var innerText = nodes[i].InnerText.Trim();
                if (!String.IsNullOrEmpty(innerText))
                {
                    Printer.WriteLine().Info2($"InnerText: {innerText}");
                }
                foreach (var a in nodes[i].Attributes)
                {
                    var value = a.Value.Trim();
                    if (value.Length > 100)
                    {
                        value = value.Substring(0, 100);
                        value += " [Truncated]";
                    }
                    Printer.WriteLine().Info2($"{a.Name}: {value}");
                }
                Printer.NewLine();
            }
        }
    }
}