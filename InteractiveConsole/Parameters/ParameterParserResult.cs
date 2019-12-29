using System.Collections.Generic;

namespace InteractiveConsole
{
    public class ParameterParserResult
    {
        private string _commandName;

        public string CommandName
        {
            get { return _commandName; }
            set
            {
                _commandName = value;
                if (_commandName.EndsWith("Command"))
                {
                    CommandNameWithoutSuffix = value.Substring(0, value.Length - "Command".Length);
                }
                else
                {
                    CommandNameWithoutSuffix = value;
                }
            }
        }
        public string CommandNameWithoutSuffix { get; private set; }
        public List<Parameter> Parameters { get; set; }
        public bool Success { get; set; }
    }
}