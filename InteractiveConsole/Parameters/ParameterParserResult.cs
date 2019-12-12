using System.Collections.Generic;

namespace InteractiveConsole
{
    public class ParameterParserResult
    {
        public string CommandName { get; set; }
        public List<Parameter> Parameters { get; set; }
        public bool Success { get; set; }
    }
}