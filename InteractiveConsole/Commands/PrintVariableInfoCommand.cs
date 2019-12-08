using System;
using InteractiveConsole.Attributes;
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
            Console.Write($"Variable {Variable} is a ");
            string typeString;
            if (Variable.IsList)
            {
                if (Variable.IsListItemCustomObject)
                {
                    typeString = $"list of {Variable.Length} {Variable.ListItemObjectName} objects";
                }
                else
                {
                    typeString = Variable switch
                    {
                        var o when o.IsListItemNumber => $"list of {Variable.Length} numbers",
                        var o when o.IsListItemString => $"list of {Variable.Length} strings",
                        _ => $"list of {Variable.Length} custom objects"
                    };
                }
            }
            else
            {
                if (Variable.IsCustomObject)
                {
                    typeString = $"{Variable.ObjectName} object";
                }
                else
                {
                    typeString = Variable switch
                    {
                        var o when o.IsList => "list",
                        var o when o.IsNumber => "number",
                        var o when o.IsString => "string",
                        _ => "object"
                    };
                }
            }

            Console.WriteLine($"{typeString} returned by {Variable.ProducedByCommand}");

            return null;
        }
    }
}