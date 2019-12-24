using System;
using InteractiveConsole.Attributes;
using InteractiveConsole.Constants;
using InteractiveConsole.Storage;

namespace InteractiveConsole.Commands
{
    [Command(description: "Prints variable type and value", Category = CommandCategories.BuiltIn)]
    public class PrintValueCommand : BaseCommand
    {
        [CommandParameter]
        [Required]
        public InMemoryStorageVariable Variable { get; set; }

        [CommandParameter]
        public int ListIndex { get; set; } = -1;

        public override object Execute()
        {
            Printer.WriteLine().Info($"Variable {Variable} is a {Variable.ToTypeString()} returned by {Variable.ProducedByCommand}");

            if (Variable.TypeInfo.IsList)
            {
                if (ListIndex > -1)
                {
                    if (ListIndex >= Variable.Length)
                    {
                        Printer.WriteLine().Error($"Index {ListIndex} is out of bounds, list length is {Variable.Length}");
                        return null;
                    }

                    var listItem = Variable.TypeInfo.Type.GetProperty("Item").GetValue(Variable.Value, new object[] { ListIndex });
                    Printer.Write().Highlight($"#{ListIndex} ");
                    Printer.WriteLine().Info2(listItem.ToString());
                }
                else
                {
                    for (var i = 0; i < Variable.Length; i++)
                    {
                        var listItem = Variable.TypeInfo.Type.GetProperty("Item").GetValue(Variable.Value, new object[] { i });
                        Printer.Write().Highlight($"#{i} ");
                        Printer.WriteLine().Info2(listItem.ToString());
                    }
                }
            }
            else if (!String.IsNullOrEmpty(Variable.ValueString.Trim()))
            {
                Printer.WriteLine().Info2(Variable.ValueString);
            }

            return null;
        }
    }
}