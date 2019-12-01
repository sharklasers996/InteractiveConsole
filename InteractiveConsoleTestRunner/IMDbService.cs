using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

namespace InteractiveConsoleTestRunner
{
    public class IMDbService : IIMDbService
    {
        private List<IMDbSearchItem> Search(string searchUrl)
        {
            var results = new List<IMDbSearchItem>();

            try
            {
                var content = DownloadString(searchUrl);

                var doc = new HtmlDocument();
                doc.LoadHtml(content);

                var titleNodes = doc.DocumentNode.SelectNodes("//h3[@class='lister-item-header']|//td[@class='result_text']");
                if (titleNodes != null)
                {
                    foreach (var node in titleNodes)
                    {
                        var urlNode = node.SelectSingleNode("./a");
                        var hrefAttribute = urlNode?.Attributes["href"];
                        if (hrefAttribute == null)
                        {
                            continue;
                        }

                        var imdbId = string.Empty;
                        var imdbIdMatch = Regex.Match(hrefAttribute.Value, @"(?<id>tt\d+)");
                        if (imdbIdMatch.Success)
                        {
                            imdbId = imdbIdMatch.Groups["id"].Value;
                        }

                        results.Add(new IMDbSearchItem
                        {
                            Title = node.InnerText.Trim(),
                            Url = NormalizeIMDbUrl("http://www.imdb.com" + hrefAttribute.Value),
                            IMDbId = imdbId
                        });
                    }
                }
            }
            catch
            {
                // Ignored
            }

            return results;
        }

        public List<IMDbSearchItem> SearchTvShows(string query)
        {
            var searchUrl =
                $"http://www.imdb.com/search/title?title={HttpUtility.UrlPathEncode(query)}&title_type=tv_series,tv_miniseries";
            return Search(searchUrl);
        }

        public List<IMDbSearchItem> SearchAll(string query)
        {
            var searchUrl = $"https://www.imdb.com/find?ref_=nv_sr_fn&q={HttpUtility.UrlPathEncode(query)}&s=all";
            return Search(searchUrl);
        }

        public List<IMDbSearchItem> SearchMovies(string query)
        {
            var searchUrl = $"http://www.imdb.com/find?q={HttpUtility.UrlPathEncode(query)}&s=tt&ttype=ft&ref_=fn_ft";
            return Search(searchUrl);
        }

        private string DownloadString(string url)
        {
            using var webClient = new WebClient();
            return webClient.DownloadString(new Uri(url));
        }

        private static string NormalizeIMDbUrl(string url)
        {
            var m = Regex.Match(url, @"(?<link>imdb\.com/(name|title)/[^/]+)/?");
            if (m.Success)
            {
                return "http://" + m.Groups["link"].Value;
            }

            return null;
        }
    }
}