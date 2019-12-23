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
            Printer.WriteLine().Info($"Variable {Variable} is a {Variable.ToTypeString()} returned by {Variable.ProducedByCommand}");

            if (Variable.TypeInfo.IsList)
            {
                var rangeTo = 5;
                if (Variable.Length < 5)
                {
                    rangeTo = Variable.Length;
                }

                Printer.WriteLine().Info($"First {rangeTo} values:");
                Printer.NewLine();
                for (var i = 0; i < rangeTo; i++)
                {
                    var listItem = Variable.TypeInfo.Type.GetProperty("Item").GetValue(Variable.Value, new object[] { i });
                    Printer.WriteLine().Info2($"#{i}: {listItem.ToString()}");
                }
            }
            else if (!String.IsNullOrEmpty(Variable.ValueString.Trim()))
            {
                Printer.WriteLine().Info("Variable value: ");
                Printer.NewLine();
                Printer.WriteLine().Info2(Variable.ValueString);
            }

            return null;
        }
    }
}