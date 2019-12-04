using System.Linq;
using System;
using Unity;
using InteractiveConsole.Output;
using InteractiveConsole.Storage;

namespace InteractiveConsole
{
    public class InteractiveConsoleRunner
    {
        public IUnityContainer Container { get; set; }
        public string Title { get; set; }

        private readonly ICommandDiscovery _commandDiscovery;
        private readonly IInputHandler _inputHandler;
        private readonly IPrinter _printer;
        private readonly IInMemoryStorage _inMemoryStorage;

        public InteractiveConsoleRunner(
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
                        ((BaseCommand)commandInstance).InputHandler = _inputHandler;
                    }

                    var parameterProcessor = new ParameterProcessor(_inMemoryStorage)
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
    }
}