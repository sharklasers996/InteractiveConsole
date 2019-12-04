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
        private readonly IUnityContainer _unityContainer;

        public void Run()
        {
            var runner = _unityContainer.Resolve<InteractiveConsoleRunner>();
            runner.Container = _unityContainer;
            runner.Title = _title;

            runner.Run();
        }

        public InteractiveConsoleBuilder()
        {
            _unityContainer = new UnityContainer();
            _unityContainer.RegisterSingleton<IInMemoryStorage, InMemoryStorage>();
            _unityContainer.RegisterType<ICommandDiscovery, CommandDiscovery>();
            _unityContainer.RegisterType<IInputHandler, InputHandler>(new InjectionConstructor(new AutoCompleteHandler(), PrinterThemes.LadiesNight));
            _unityContainer.RegisterType<IPrinter, Printer>();
        }

        public InteractiveConsoleBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public InteractiveConsoleBuilder WithServices(Action<IUnityContainer> containerFunc)
        {
            containerFunc.Invoke(_unityContainer);
            return this;
        }
    }
}