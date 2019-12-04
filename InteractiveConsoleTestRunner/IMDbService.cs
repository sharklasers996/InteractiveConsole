using System;
using System.Collections.Generic;
using System.Linq;
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

        public IMDbTitleData GetData(string url)
        {
            var result = new IMDbTitleData
            {
                Url = url
            };

            try
            {
                var content = DownloadString(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(content.Trim());

                var titleNode = doc.DocumentNode.SelectSingleNode("//div[@class='title_wrapper']/h1");
                result.Title = titleNode?.InnerText;

                var yearNode = doc.DocumentNode.SelectSingleNode("//span[@id='titleYear']");
                result.Year = yearNode?.InnerText;

                var posterNodes = doc.DocumentNode.SelectNodes("//img[contains(@title, 'Poster')]");
                var posterSrc = posterNodes?[0].Attributes["src"];
                if (posterSrc != null)
                {
                    var posterValue = posterSrc.Value;

                    var posterEnding = "V1_";
                    var endIndex = posterValue.IndexOf(posterEnding) + posterEnding.Length;
                    if (endIndex <= posterValue.Length)
                    {
                        result.PosterUrl = posterValue.Substring(0, endIndex) + ".jpg";
                    }
                    else
                    {
                        result.PosterUrl = posterValue.Replace("SX214_AL_.jpg", "SX640_SY720_.jpg");
                    }
                }

                var ratingNode = doc.DocumentNode.SelectSingleNode("//span[@itemprop='ratingValue']");
                if (ratingNode != null)
                {
                    result.Rating = Convert.ToDouble(ratingNode.InnerText);
                }

                var plotNode = doc.DocumentNode.SelectSingleNode("//div[@class='summary_text']");
                if (plotNode != null)
                {
                    result.Plot = HttpUtility.HtmlDecode(plotNode.InnerText.Trim());
                }

                var genreNodes = doc.DocumentNode.SelectNodes("//div[@class='subtext']/a[contains(@href, 'genre')]");
                if (genreNodes != null)
                {
                    foreach (var genreNode in genreNodes)
                    {
                        result.Genres += genreNode.InnerText.Trim() + " | ";
                    }

                    result.Genres = result.Genres.Trim(' ', '|');
                }

                var actorsNodes = doc.DocumentNode.SelectNodes("//div[@class='plot_summary ']//div[4]/a[contains(@href, '/name/')]");
                if (actorsNodes != null)
                {
                    result.Actors = string.Join(", ", actorsNodes.Select(x => x.InnerText));
                }

                var durationNode = doc.DocumentNode.SelectSingleNode("//div[@class='subtext']/time");
                if (durationNode != null)
                {
                    result.Duration = durationNode.InnerText.Trim();
                }

                var episodeGuideNode = doc.DocumentNode.SelectSingleNode("//a[@class='bp_item np_episode_guide np_right_arrow']");
                if (episodeGuideNode != null)
                {
                    result.IsTvShow = true;
                }
            }
            catch
            {
                // Ignored
            }

            return result;
        }
    }
}