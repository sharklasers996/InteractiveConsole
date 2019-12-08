using System.Collections.Generic;

namespace InteractiveConsole.Models
{
    public class CommandOptionInfo
    {
        public string Name { get; set; }
        public bool Required { get; set; }
        public List<string> AvailableValues { get; set; }
        public bool IsNumber { get; set; }
        public bool IsString { get; set; }
        public bool IsEnum { get; set; }
        public bool IsCustomObject { get; set; }
        public string ObjectName { get; set; }

        public bool IsList { get; set; }
        public bool IsListItemNumber { get; set; }
        public bool IsListItemString { get; set; }
        public bool IsListItemEnum { get; set; }
        public bool IsListItemCustomObject { get; set; }
        public string ListItemObjectName { get; set; }
    }
}