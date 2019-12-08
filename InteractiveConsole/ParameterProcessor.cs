using System.Reflection;
using System.Linq;
using System;
using InteractiveConsole.Models;
using InteractiveConsole.Extensions;
using InteractiveConsole.Storage;

namespace InteractiveConsole
{
    public class ParameterProcessor
    {
        public BaseCommand CommandInstance { get; set; }
        public ParameterParserResult ParserResult { get; set; }
        public CommandInfo CommandInfo { get; set; }

        private readonly IInMemoryStorage _InMemoryStorage;

        public ParameterProcessor(IInMemoryStorage InMemoryStorage)
        {
            _InMemoryStorage = InMemoryStorage;
        }

        public bool SetParameters()
        {
            // Console.WriteLine($"Command: {ParserResult.CommandName}");
            // if (ParserResult.Parameters != null)
            // {
            //     foreach (var param in ParserResult.Parameters)
            //     {
            //         Console.WriteLine($"{param.Name}={param.Value}");
            //     }
            // }

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
                var inMemoryVariable = _InMemoryStorage.TryGetVariable(parameter.Value);
                if (inMemoryVariable != null)
                {
                    if (!SetInMemoryVariableValue(instanceProperty, inMemoryVariable))
                    {
                        return false;
                    }
                }
                else if (!SetParameterValue(instanceProperty, parameter))
                {
                    return false;
                }
            }

            return true;
        }

        private bool SetInMemoryVariableValue(PropertyInfo instanceProperty, InMemoryStorageVariable inMemoryVariable)
        {
            if (instanceProperty.PropertyType.IsNumericType()
                && inMemoryVariable.IsNumber)
            {
                instanceProperty.SetValue(CommandInstance, inMemoryVariable.Value.ToString());
            }
            else if (instanceProperty.PropertyType == typeof(InMemoryStorageVariable))
            {
                instanceProperty.SetValue(CommandInstance, inMemoryVariable);
            }
            else
            {
                if (instanceProperty.PropertyType != inMemoryVariable.GetType())
                {
                    Console.WriteLine("Parameter type does not match");
                    return false;
                }

                instanceProperty.SetValue(CommandInstance, inMemoryVariable.Value);
            }

            return true;
        }

        private bool SetParameterValue(PropertyInfo instanceProperty, Parameter parameter)
        {
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

            if (instanceProperty.PropertyType != parameterValue.GetType())
            {
                Console.WriteLine("Parameter type does not match");
                return false;
            }

            instanceProperty.SetValue(CommandInstance, parameterValue);
            return true;
        }
    }
}