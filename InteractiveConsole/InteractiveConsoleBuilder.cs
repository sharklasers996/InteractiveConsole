using System;
using InteractiveConsole.Output;
using InteractiveConsole.Storage;
using Unity;
using Unity.Injection;

namespace InteractiveConsole
{
    public class InteractiveConsoleBuilder
    {
        private string _title;
        private bool _titleAscii;
        private string _welcomeText;
        private readonly IUnityContainer _unityContainer;
        private PrinterTheme _theme = PrinterThemes.Default;
        private bool _printAvailableCommandsOnStart = true;

        private void ConfigureContainer()
        {
            _unityContainer.RegisterSingleton<IInMemoryStorage, InMemoryStorage>();
            _unityContainer.RegisterType<ICommandDiscovery, CommandDiscovery>();
            _unityContainer.RegisterType<ITypeProvider, TypeProvider>();
            _unityContainer.RegisterType<IAutoCompleteHandler, AutoCompleteHandler>();
            _unityContainer.RegisterType<ICommandReader, CommandReader>();
            _unityContainer.RegisterType<IPrinter, Printer>(new InjectionConstructor(_theme));
            _unityContainer.RegisterType<IReader, Reader>(new InjectionConstructor(_theme));
        }

        public InteractiveConsoleBuilder()
        {
            _unityContainer = new UnityContainer();
        }

        public void Run()
        {
            ConfigureContainer();

            var runner = _unityContainer.Resolve<InteractiveConsoleRunner>();
            runner.Container = _unityContainer;
            runner.Title = _title;
            runner.TitleAscii = _titleAscii;
            runner.PrintAvailableCommandsOnStart = _printAvailableCommandsOnStart;
            runner.WelcomeText = _welcomeText;

            runner.Run();
        }

        public InteractiveConsoleBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public InteractiveConsoleBuilder WithAsciiTitle(string title)
        {
            _title = title;
            _titleAscii = true;
            return this;
        }

        public InteractiveConsoleBuilder WithServices(Action<IUnityContainer> containerFunc)
        {
            containerFunc.Invoke(_unityContainer);
            return this;
        }

        public InteractiveConsoleBuilder WithTheme(PrinterTheme theme)
        {
            _theme = theme;
            return this;
        }

        public InteractiveConsoleBuilder DoNotPrintCommandsOnStart()
        {
            _printAvailableCommandsOnStart = false;
            return this;
        }

        public InteractiveConsoleBuilder WithWelcomeText(string text)
        {
            _welcomeText = text;
            return this;
        }
    }
}