using System.Collections.Generic;
using System.Threading.Tasks;

namespace InteractiveConsoleTestRunner
{
    public interface IIMDbService
    {
        List<IMDbSearchItem> SearchAll(string query);
        List<IMDbSearchItem> SearchMovies(string query);
        List<IMDbSearchItem> SearchTvShows(string query);
    }
}