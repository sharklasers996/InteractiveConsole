using System.Reflection;
using System.Linq;
using System;
using InteractiveConsole.Models;
using InteractiveConsole.Extensions;
using InteractiveConsole.Storage;
using System.Collections.Generic;

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

        public List<string> SetParameters()
        {
            // Console.WriteLine($"Command: {ParserResult.CommandName}");
            // if (ParserResult.Parameters != null)
            // {
            //     foreach (var param in ParserResult.Parameters)
            //     {
            //         Console.WriteLine($"{param.Name}={param.Value}");
            //     }
            // }
            var errors = new List<string>();

            var commandType = CommandInstance.GetType();
            foreach (var option in CommandInfo.Options.OrderBy(x => x.Required))
            {
                var parameter = ParserResult.Parameters?.FirstOrDefault(x => x.Name == option.Name);
                if (parameter == null)
                {
                    if (option.Required)
                    {
                        errors.Add($"Option '{option.Name}' is required");
                        return errors;
                    }

                    continue;
                }

                var instanceProperty = commandType.GetProperty(parameter.Name);
                var inMemoryVariable = _InMemoryStorage.TryGetVariable(parameter.Value);
                if (inMemoryVariable != null)
                {
                    var inMemoryVarSetErrors = SetInMemoryVariableValue(instanceProperty, inMemoryVariable);
                    if (inMemoryVarSetErrors.Any())
                    {
                        return inMemoryVarSetErrors;
                    }
                }
                else
                {
                    var paramSetErrors = SetParameterValue(instanceProperty, parameter);
                    if (paramSetErrors.Any())
                    {
                        return paramSetErrors;
                    }
                }
            }

            return errors;
        }

        private List<string> SetInMemoryVariableValue(PropertyInfo instanceProperty, InMemoryStorageVariable inMemoryVariable)
        {
            var errors = new List<string>();

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
                    errors.Add("Parameter type does not match");
                }
                else
                {
                    instanceProperty.SetValue(CommandInstance, inMemoryVariable.Value);
                }
            }

            return errors;
        }

        private List<string> SetParameterValue(PropertyInfo instanceProperty, Parameter parameter)
        {
            var errors = new List<string>();
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
                errors.Add("Parameter type does not match");
            }
            else
            {
                instanceProperty.SetValue(CommandInstance, parameterValue);
            }

            return errors;
        }
    }
}