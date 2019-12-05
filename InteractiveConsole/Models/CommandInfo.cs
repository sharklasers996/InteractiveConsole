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
                NameWithoutSuffix = value.Substring(0, value.Length - "Command".Length);
            }
        }
        public string NameWithoutSuffix { get; private set; }
        public List<CommandOptionInfo> Options { get; set; }
        public Type Type { get; set; }
    }
}