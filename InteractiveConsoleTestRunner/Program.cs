using System;
using InteractiveConsole;
using Unity;

namespace InteractiveConsoleTestRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var cliRunner = new CliRunner
            {
                Container = new UnityContainer(),
                Title = "IMDb Thing"
            };
            cliRunner.Container.RegisterType<IIMDbService, IMDbService>();
            cliRunner.Run();
        }
    }
}
