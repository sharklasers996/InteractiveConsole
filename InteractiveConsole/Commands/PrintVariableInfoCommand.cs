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
        public object Variable { get; set; }

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


            Printer.Write().Info($"Variable {Variable} is a ");
            string typeString;
            if (variable.IsList)
            {
                if (variable.IsListItemCustomObject)
                {
                    typeString = $"list of {variable.Length} {variable.ListItemObjectName} objects";
                }
                else
                {
                    typeString = variable switch
                    {
                        var o when o.IsListItemNumber => $"list of {variable.Length} numbers",
                        var o when o.IsListItemString => $"list of {variable.Length} strings",
                        _ => $"list of {variable.Length} custom objects"
                    };
                }
            }
            else
            {
                if (variable.IsCustomObject)
                {
                    typeString = $"{variable.ObjectName} object";
                }
                else
                {
                    typeString = variable switch
                    {
                        var o when o.IsList => "list",
                        var o when o.IsNumber => "number",
                        var o when o.IsString => "string",
                        _ => "object"
                    };
                }
            }

            Printer.WriteLine().Info2($"{typeString} returned by {variable.ProducedByCommand}");

            return null;
        }
    }
}