using InteractiveConsole.Attributes;
using InteractiveConsole.Constants;
using InteractiveConsole.Extensions;
using InteractiveConsole.Storage;

namespace InteractiveConsole.Commands
{
    [Command(Category = CommandCategories.BuiltIn)]
    public class CreateVariableCommand : BaseCommand
    {
        [CommandParameter]
        public string Value { get; set; }

        private readonly IInMemoryStorage _inMemoryStorage;

        public CreateVariableCommand(IInMemoryStorage inMemoryStorage)
        {
            _inMemoryStorage = inMemoryStorage;
        }

        public override object Execute()
        {
            var variable = _inMemoryStorage.Add(Value, "User");

            Printer.Write().Info($"{variable.ToTypeString().ToFirstUpper()} added to storage @ ");
            Printer.WriteLine().Highlight($"#{variable.Id}");

            return null;
        }
    }
}