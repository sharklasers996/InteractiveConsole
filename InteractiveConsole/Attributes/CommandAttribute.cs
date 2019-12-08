using System;

namespace InteractiveConsole.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        public string Description { get; set; }
        public CommandAttribute(string description = null)
        {
            Description = description;
        }
    }
}