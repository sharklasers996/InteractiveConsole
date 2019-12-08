namespace InteractiveConsole.Storage
{
    public class InMemoryStorageVariable
    {
        public int Id { get; set; }
        public object Value { get; set; }
        public string ValueString { get { return Value.ToString(); } }
        public bool IsCustomObject { get; set; }
        public string ObjectName { get; set; }
        public bool IsList { get; set; }
        public bool IsListItemString { get; set; }
        public bool IsListItemNumber { get; set; }
        public bool IsListItemCustomObject { get; set; }
        public string ListItemObjectName { get; set; }
        public int Length { get; set; }
        public bool IsString { get; set; }
        public bool IsNumber { get; set; }
        public string ProducedByCommand { get; set; }

        public override string ToString()
        {
            return $"#{Id}";
        }
    }
}