using System.Linq;
using System;
using InteractiveConsole.Models;
using InteractiveConsole.Extensions;

namespace InteractiveConsole
{
    public class ParameterProcessor
    {
        public ICommand CommandInstance { get; set; }
        public ParameterParserResult ParserResult { get; set; }
        public CommandInfo CommandInfo { get; set; }

        public bool SetParameters()
        {
            Console.WriteLine($"Command: {ParserResult.CommandName}");
            if (ParserResult.Parameters != null)
            {
                foreach (var param in ParserResult.Parameters)
                {
                    Console.WriteLine($"{param.Name}={param.Value}");
                }
            }

            var commandType = CommandInstance.GetType();
            foreach (var option in CommandInfo.Options.OrderBy(x => x.Required))
            {
                var parameter = ParserResult.Parameters?.FirstOrDefault(x => x.Name == option.Name);
                if (parameter == null)
                {
                    if (option.Required)
                    {
                        Console.WriteLine($"Option '{option.Name}' is required");
                        return false;
                    }

                    continue;
                }

                var instanceProperty = commandType.GetProperty(parameter.Name);
                object parameterValue = parameter.Value;

                if (instanceProperty.PropertyType.IsNumericType()
                    && int.TryParse(parameter.Value, out var numberParameter))
                {
                    parameterValue = numberParameter;
                }

                if (instanceProperty.PropertyType.IsEnum)
                {
                    if (Enum.TryParse(instanceProperty.PropertyType, parameterValue.ToString(), out var parsedEnum))
                    {
                        parameterValue = parsedEnum;
                    }
                }

                instanceProperty.SetValue(CommandInstance, parameterValue);
            }

            return true;
        }
    }
}