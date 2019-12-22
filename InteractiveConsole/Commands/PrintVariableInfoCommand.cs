using System;
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
        [Required]
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

            Printer.WriteLine().Info($"Variable {Variable} is a {variable.ToTypeString()} returned by {variable.ProducedByCommand}");

            if (Variable.TypeInfo.IsList)
            {
                Printer.WriteLine().Info("Variable contents: ");
                Printer.NewLine();
                for (var i = 0; i < Variable.Length; i++)
                {
                    var listItem = Variable.TypeInfo.Type.GetProperty("Item").GetValue(Variable.Value, new object[] { i });
                    Printer.WriteLine().Info2($"#{i}: {listItem.ToString()}");
                    Printer.NewLine();
                }
            }
            else if (!String.IsNullOrEmpty(Variable.ValueString.Trim()))
            {
                Printer.WriteLine().Info("Variable content: ");
                Printer.NewLine();
                Printer.WriteLine().Info2(Variable.ValueString);
            }

            return null;
        }
    }
}