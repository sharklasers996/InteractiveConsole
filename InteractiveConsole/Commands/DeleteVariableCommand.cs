using InteractiveConsole.Attributes;
using InteractiveConsole.Constants;
using InteractiveConsole.Models.Storage;
using InteractiveConsole.Storage;

namespace InteractiveConsole.Commands
{
    [Command(Description = "Deletes variable from storage", Category = CommandCategories.BuiltIn)]
    public class DeleteVariableCommand : BaseCommand
    {
        [CommandParameter]
        public InMemoryStorageVariable Variable { get; set; }

        [CommandParameter]
        public int RangeFrom { get; set; }

        [CommandParameter]
        public int RangeTo { get; set; }

        private readonly IInMemoryStorage _inMemoryStorage;

        public DeleteVariableCommand(IInMemoryStorage inMemoryStorage)
        {
            _inMemoryStorage = inMemoryStorage;
        }

        public override bool IsValid()
        {
            if (Variable == null
                && RangeFrom == 0
                && RangeTo == 0)
            {
                return false;
            }

            return true;
        }

        public override object Execute()
        {
            if (Variable != null)
            {
                DeleteVariable(Variable.Id);
                return null;
            }

            for (var i = RangeFrom; i <= RangeTo; i++)
            {
                DeleteVariable(i);
            }

            return null;
        }

        private void DeleteVariable(int id)
        {
            var result = _inMemoryStorage.DeleteVariable(id);
            if (result)
            {
                Printer.WriteLine().Success($"Deleted variable #{id}");
            }
            else
            {
                Printer.WriteLine().Error($"Failed to delete variable #{id}");
            }
        }
    }
}