using InteractiveConsole;
using Unity;

namespace InteractiveConsoleTestRunner
{
    class Program
    {
        static void Main()
        {
            new InteractiveConsoleBuilder()
                .WithTitle("Video Search")
                .WithServices(services =>
                {
                    services.RegisterType<IIMDbService, IMDbService>();
                    services.RegisterSingleton<ITorrentService, TorrentService>();
                }).Run();
        }
    }
}
