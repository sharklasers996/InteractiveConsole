using System.Collections;
using InteractiveConsole.Attributes;
using InteractiveConsole.Extensions;
using InteractiveConsole.Storage;

namespace InteractiveConsole.Commands
{
    [Command(description: "Prints variable's type and value")]
    public class PrintVariableInfoCommand : BaseCommand
    {
        [CommandParameter]
        public InMemoryStorageVariable Variable { get; set; }

        public override object Execute()
        {
            if (!(Variable is InMemoryStorageVariable variable))
            {
                if (Variable.GetType().IsList())
                {
                    foreach (var v in (IList)Variable)
                    {
                        Printer.WriteLine().Info(v.ToString());
                    }
                }
                else
                {
                    Printer.WriteLine().Info(Variable.ToString());
                }
                return null;
            }

            Printer.WriteLine().Info($"Variable {Variable} is a {variable.ToTypeString()}");

            return null;
        }
    }
}