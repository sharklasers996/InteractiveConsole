using System;
using System.Collections.Generic;

namespace InteractiveConsole.Models
{
    public class CommandInfo
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NameWithoutSuffix = value.Replace("Command", string.Empty);
            }
        }
        public string NameWithoutSuffix { get; private set; }
        public List<CommandOptionInfo> Options { get; set; }
        public Type Type { get; set; }
    }
}