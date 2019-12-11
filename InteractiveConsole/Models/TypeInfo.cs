namespace InteractiveConsole.Models
{
    public class TypeInfo
    {
        public bool IsNumber { get; set; }
        public bool IsString { get; set; }
        public bool IsEnum { get; set; }
        public bool IsCustomObject { get; set; }
        public string ObjectName { get; set; }
        public bool IsBool { get; set; }

        public bool IsList { get; set; }
        public bool IsListItemNumber { get; set; }
        public bool IsListItemString { get; set; }
        public bool IsListItemCustomObject { get; set; }
        public string ListItemObjectName { get; set; }
    }
}