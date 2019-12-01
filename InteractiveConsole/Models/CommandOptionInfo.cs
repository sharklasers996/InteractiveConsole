using System.Collections.Generic;

namespace InteractiveConsole.Models
{
    public class CommandOptionInfo
    {
        public string Name { get; set; }
        public bool Required { get; set; }
        public List<string> AvailableValues { get; set; }
    }
}