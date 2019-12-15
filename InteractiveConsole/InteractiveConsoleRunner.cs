using System.Linq;
using System;
using Unity;
using InteractiveConsole.Output;
using InteractiveConsole.Storage;
using InteractiveConsole.Models;
using System.Collections.Generic;

namespace InteractiveConsole
{
    public class InteractiveConsoleRunner
    {
        public IUnityContainer Container { get; set; }
        public string Title { get; set; }

        private readonly ICommandDiscovery _commandDiscovery;
        private readonly IPrinter _printer;
        private readonly IInMemoryStorage _inMemoryStorage;
        private readonly ICommandReader _commandReader;

        public InteractiveConsoleRunner(
            ICommandDiscovery commandDiscovery,
            IInMemoryStorage inMemoryStorage,
            IPrinter printer,
            ICommandReader commandReader)
        {
            _commandReader = commandReader;
            _commandDiscovery = commandDiscovery;
            _printer = printer;
            _inMemoryStorage = inMemoryStorage;
        }

        public void Run()
        {
            if (!String.IsNullOrEmpty(Title))
            {
                _printer.NewLine();
                _printer.Ascii(Title);
                _printer.NewLine();
            }
            _printer.Print(_commandDiscovery.AvailableCommands);

            while (true)
            {
                try
                {
                    var input = _commandReader.ReadLine();
                    var parserResult = ParameterParser.Parse(input);

                    if (!TryGetCommand(parserResult, out var command))
                    {
                        _printer.WriteLine().Error("Could not find command");
                        continue;
                    }

                    if (!TryCreateCommandInstance(command, out var commandInstance))
                    {
                        _printer.WriteLine().Error("Could not create command instance");
                        continue;
                    }

                    if (!TrySetParameters(commandInstance, parserResult, command, out var error))
                    {
                        _printer.WriteLine().Error($"Failed to set parameters: {error}");
                        continue;
                    }

                    if (!commandInstance.IsValid())
                    {
                        _printer.WriteLine().Error("Command options are invalid");
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
                    _printer.WriteLine().Error($"Error: {ex.Message}\n{ex.StackTrace}");
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
                commandInstance.Reader = Container.Resolve<IReader>();

                return true;
            }

            return false;
        }

        private bool TrySetParameters(BaseCommand commandInstance, ParameterParserResult parserResult, CommandInfo command, out string error)
        {
            var parameterProcessor = new ParameterProcessor(_inMemoryStorage)
            {
                CommandInstance = commandInstance,
                ParserResult = parserResult,
                CommandInfo = command
            };

            error = parameterProcessor.SetParameters();

            return String.IsNullOrEmpty(error);
        }
    }
}