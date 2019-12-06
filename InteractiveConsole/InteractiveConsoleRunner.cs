using System.Linq;
using System;
using Unity;
using InteractiveConsole.Output;
using InteractiveConsole.Storage;
using InteractiveConsole.Models;

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
                try
                {
                    var input = _inputHandler.ReadLine();
                    var parserResult = ParameterParser.Parse(input);

                    if (!TryGetCommand(parserResult, out var command))
                    {
                        _printer.Print("Could not find command");
                        continue;
                    }

                    if (!TryCreateCommandInstance(command, out var commandInstance))
                    {
                        _printer.Print("Could not create command instance");
                        continue;
                    }

                    if (!commandInstance.IsValid())
                    {
                        _printer.Print("Command options are invalid");
                        continue;
                    }

                    if (!TrySetParameters(commandInstance, parserResult, command))
                    {
                        _printer.Print("Failed to set parameters");
                        continue;
                    }

                    var result = commandInstance.Execute();
                    if (result != null)
                    {
                        _inMemoryStorage.Add(result, parserResult);
                    }
                }
                catch (Exception ex)
                {
                    _printer.Print($"Error: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        private bool TryGetCommand(ParameterParserResult parserResult, out CommandInfo command)
        {
            command = _commandDiscovery
               .AvailableCommands
               .FirstOrDefault(x => x.Name == parserResult.CommandName);

            return command != null;
        }

        private bool TryCreateCommandInstance(CommandInfo command, out BaseCommand commandInstance)
        {
            commandInstance = Container.Resolve(command.Type) as BaseCommand;
            if (commandInstance != null)
            {
                commandInstance.Printer = _printer;
                commandInstance.InputHandler = _inputHandler;

                return true;
            }

            return false;
        }

        private bool TrySetParameters(BaseCommand commandInstance, ParameterParserResult parserResult, CommandInfo command)
        {
            var parameterProcessor = new ParameterProcessor(_inMemoryStorage)
            {
                CommandInstance = commandInstance,
                ParserResult = parserResult,
                CommandInfo = command
            };

            return parameterProcessor.SetParameters();
        }
    }
}