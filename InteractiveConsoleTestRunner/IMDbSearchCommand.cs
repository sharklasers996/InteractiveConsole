using System.Collections.Generic;
using InteractiveConsole.Attributes;
using InteractiveConsole;

namespace InteractiveConsoleTestRunner
{
    [Command]
    public class IMDbSearchCommand : BaseCommand, ICommand
    {
        [CommandParameter]
        [Required]
        public string Query { get; set; }

        [CommandParameter]
        [Required]
        public IMDbSearchCategory Category { get; set; }

        private readonly IIMDbService _imdbService;

        public IMDbSearchCommand(IIMDbService imdbService)
        {
            _imdbService = imdbService;
        }

        public object Execute()
        {
            Printer.PrintHeader("Executing IMDb search...");
            var results = new List<IMDbSearchItem>();

            switch (Category)
            {
                case IMDbSearchCategory.All:
                    results = _imdbService.SearchAll(Query);
                    break;
                case IMDbSearchCategory.Movies:
                    results = _imdbService.SearchMovies(Query);
                    break;
                case IMDbSearchCategory.TvShows:
                    results = _imdbService.SearchTvShows(Query);
                    break;
            }
            Printer.PrintSubheader($"Found {results.Count} results");
            Printer.Print();

            foreach (var r in results)
            {
                var selection = Printer.PrintWithSelection(r, new Dictionary<string, string> { { "n", "Next" }, { "q", "Quit" } });
                if (selection == "q")
                {
                    break;
                }
            }

            Printer.PrintSubheader("Search completed.");

            return results;
        }
    }
}