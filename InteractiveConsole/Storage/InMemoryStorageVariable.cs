using InteractiveConsole.Models;

namespace InteractiveConsole.Storage
{
    public class InMemoryStorageVariable
    {
        public int Id { get; set; }
        public object Value { get; set; }
        public string ValueString { get { return Value.ToString(); } }
        public string ProducedByCommand { get; set; }
        public int Length { get; set; }
        public TypeInfo TypeInfo { get; set; }

        public override string ToString()
        {
            return $"#{Id}";
        }

        public string ToTypeString()
        {
            string typeString;
            if (TypeInfo.IsList)
            {
                if (TypeInfo.IsListItemCustomObject)
                {
                    typeString = $"list of {Length} {TypeInfo.ListItemObjectName} objects";
                }
                else
                {
                    typeString = TypeInfo switch
                    {
                        var o when o.IsListItemNumber => $"list of {Length} numbers",
                        var o when o.IsListItemString => $"list of {Length} strings",
                        _ => $"list of {Length} custom objects"
                    };
                }
            }
            else
            {
                if (TypeInfo.IsCustomObject)
                {
                    typeString = $"{TypeInfo.ObjectName} object";
                }
                else
                {
                    typeString = TypeInfo switch
                    {
                        var o when o.IsList => "list",
                        var o when o.IsNumber => "number",
                        var o when o.IsString => "string",
                        _ => "object"
                    };
                }
            }

            return $"{typeString}";
        }
    }
}