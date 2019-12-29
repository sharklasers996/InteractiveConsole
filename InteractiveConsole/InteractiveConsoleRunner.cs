using System.Linq;
using System;
using Unity;
using InteractiveConsole.Output;
using InteractiveConsole.Storage;
using InteractiveConsole.Models;
using InteractiveConsole.Commands;
using InteractiveConsole.Extensions;

namespace InteractiveConsole
{
    public class InteractiveConsoleRunner
    {
        public IUnityContainer Container { get; set; }
        public string Title { get; set; }
        public bool TitleAscii { get; set; }
        public string WelcomeText { get; set; }
        public bool PrintAvailableCommandsOnStart { get; set; }

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
            PrintTitle();
            PrintWelcomeText();
            PrintCommands();

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

                    if (!TryCreateCommandInstance(command.Type, out var commandInstance))
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

                    _printer.NewLine();
                    var result = commandInstance.Execute();
                    if (result != null)
                    {
                        var variable = _inMemoryStorage.Add(result, parserResult);
                        _printer.Write().Info($"{variable.ToTypeString().ToFirstUpper()} added to storage @ ");
                        _printer.WriteLine().Highlight($"#{variable.Id}");
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

        private bool TryCreateCommandInstance(Type commandType, out BaseCommand commandInstance)
        {
            commandInstance = Container.Resolve(commandType) as BaseCommand;
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

        private void PrintWelcomeText()
        {
            if (!String.IsNullOrEmpty(WelcomeText))
            {
                _printer.WriteLine().Info(WelcomeText);
                _printer.NewLine();
            }
        }

        private void PrintTitle()
        {
            if (!String.IsNullOrEmpty(Title))
            {
                _printer.NewLine();
                if (TitleAscii)
                {
                    _printer.WriteLine().Ascii(Title);
                }
                else
                {
                    _printer.WriteLine().Highlight(Title);
                }
            }
        }

        private void PrintCommands()
        {
            if (!PrintAvailableCommandsOnStart)
            {
                return;
            }

            if (TryCreateCommandInstance(typeof(PrintCommandsCommand), out var command))
            {
                command.Execute();
            }
        }
    }
}