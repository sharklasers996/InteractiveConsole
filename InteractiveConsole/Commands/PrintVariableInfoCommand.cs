using InteractiveConsole.Attributes;
using InteractiveConsole.Storage;

namespace InteractiveConsole.Commands
{
    [Command]
    public class PrintVariableInfoCommand : BaseCommand, ICommand
    {
        [CommandParameter]
        public InMemoryStorageVariable Variable { get; set; }

        public object Execute()
        {
            if (Variable.IsList)
            {
                Printer.Print($"Variable {Variable} is a list containing {Variable.Length} items.");
            }
            else if (Variable.IsNumber)
            {
                Printer.Print($"Varialbe {Variable} is a number with a value of {Variable.ValueString}");
            }
            else if (Variable.IsString)
            {
                Printer.Print($"Variable {Variable} is a string with a value of '{Variable.ValueString}'");
            }
            else
            {
                Printer.Print($"Variable {Variable} is an object with a value of {Variable.ValueString}");
            }

            return null;
        }
    }
}