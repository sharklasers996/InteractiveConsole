using System.Linq;
using System;
using Unity;
using InteractiveConsole.Output;
using InteractiveConsole.Storage;
using Unity.Injection;
using System.Collections.Generic;

namespace InteractiveConsole
{
    public class CliRunner
    {
        public IUnityContainer Container { get; set; }
        public string Title { get; set; }

        private ICommandDiscovery _commandDiscovery;
        private IInputHandler _inputHandler;
        private IPrinter _printer;
        private IInMemoryStorage _inMemoryStorage;

        public CliRunner()
        {
        }

        public CliRunner(
            ICommandDiscovery commandDiscovery,
            IInputHandler inputHandler,
            IPrinter printer,
            IInMemoryStorage inMemoryStorage
        )
        {
            _commandDiscovery = commandDiscovery;
            _inputHandler = inputHandler;
            _printer = printer;
            _inMemoryStorage = inMemoryStorage;
        }

        public void Run()
        {
            ConfigureContainer();
            // TODO: figure out flow for testing instead of this
            ResolveDependencies();

            if (!String.IsNullOrEmpty(Title))
            {
                _printer.PrintAscii(Title, PrinterFonts.Big);
            }
            _printer.PrintCommands(_commandDiscovery.AvailableCommands);

            while (true)
            {
                var input = _inputHandler.ReadLine();
                var parserResult = ParameterParser.Parse(input);

                var command = _commandDiscovery
                    .AvailableCommands
                    .FirstOrDefault(x => x.Name == parserResult.CommandName);
                if (command != null)
                {
                    if (!(Container.Resolve(command.Type) is ICommand commandInstance))
                    {
                        Console.WriteLine("Failed to create command instance");
                        continue;
                    }

                    if (command.Type.IsSubclassOf(typeof(BaseCommand)))
                    {
                        ((BaseCommand)commandInstance).Printer = _printer;
                    }

                    var parameterProcessor = new ParameterProcessor
                    {
                        CommandInstance = commandInstance,
                        ParserResult = parserResult,
                        CommandInfo = command
                    };

                    var success = parameterProcessor.SetParameters();
                    if (success)
                    {
                        var result = commandInstance.Execute();
                        if (result != null)
                        {
                            _inMemoryStorage.Add(result, parserResult);
                        }
                    }
                }
            }
        }

        private void ConfigureContainer()
        {
            if (Container == null)
            {
                Container = new UnityContainer();
            }

            Container.RegisterSingleton<IInMemoryStorage, InMemoryStorage>();
            Container.RegisterType<ICommandDiscovery, CommandDiscovery>();
            Container.RegisterType<IInputHandler, InputHandler>(new InjectionConstructor(new AutoCompleteHandler(), PrinterThemes.LadiesNight));
            Container.RegisterType<IPrinter, Printer>();
        }

        private void ResolveDependencies()
        {
            _commandDiscovery = Container.Resolve<ICommandDiscovery>();
            _inputHandler = Container.Resolve<IInputHandler>();
            _printer = Container.Resolve<IPrinter>();
            _inMemoryStorage = Container.Resolve<IInMemoryStorage>();
        }
    }
}