using System;

namespace InteractiveConsole.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        public string Description { get; set; }
        public string Category { get; set; }

        public CommandAttribute(string description = null, string category = null)
        {
            Description = description;
            Category = category;
        }
    }
}