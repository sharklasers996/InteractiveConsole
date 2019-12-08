using System.Reflection;
using System.Linq;
using System;
using InteractiveConsole.Models;
using InteractiveConsole.Extensions;
using InteractiveConsole.Storage;
using System.Collections.Generic;
using System.Collections;

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
                    if (parameter.IndexFrom != null)
                    {
                        var inMemoryVarSetErrors = SetInMemoryVariableIndexValue(instanceProperty, inMemoryVariable, (int)parameter.IndexFrom, parameter.IndexTo);
                        if (inMemoryVarSetErrors.Any())
                        {
                            return inMemoryVarSetErrors;
                        }
                    }
                    else
                    {
                        var inMemoryVarSetErrors = SetInMemoryVariableValue(instanceProperty, inMemoryVariable);
                        if (inMemoryVarSetErrors.Any())
                        {
                            return inMemoryVarSetErrors;
                        }
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

        private List<string> SetInMemoryVariableIndexValue(PropertyInfo instanceProperty, InMemoryStorageVariable inMemoryVariable, int indexFrom, int? indexTo)
        {
            if (!inMemoryVariable.IsList)
            {
                return new List<string> { "Index can only be used on list" };
            }

            if (instanceProperty.PropertyType != inMemoryVariable.Value.GetType())
            {
                return new List<string> { "Parameter type does not match" };
            }

            var listType = inMemoryVariable.Value.GetType();
            var listItemType = listType.GetListItemType();
            var instancedList = (IList)typeof(List<>)
                .MakeGenericType(listItemType)
                .GetConstructor(System.Type.EmptyTypes)
                .Invoke(null);

            int listLength = (int)listType.GetProperty("Count").GetValue(inMemoryVariable.Value, null);

            if (indexFrom > listLength)
            {
                return new List<string> { "Index is out of bounds" };
            }

            if (indexTo != null
                && indexTo >= listLength)
            {
                return new List<string> { "Index is out of bounds" };
            }

            for (var i = 0; i < listLength; i++)
            {
                if (i < indexFrom)
                {
                    continue;
                }

                var listItemValue = listType.GetProperty("Item").GetValue(inMemoryVariable.Value, new object[] { i });
                var listItemValueType = listItemValue.GetType();

                object value;
                if (instanceProperty.PropertyType.GetListItemType().IsNumericType()
                    && listItemValueType.IsNumericType())
                {
                    value = int.Parse(listItemValue.ToString());
                }
                else if (instanceProperty.PropertyType == typeof(InMemoryStorageVariable))
                {
                    value = listItemValue;
                }
                else
                {
                    if (instanceProperty.PropertyType != inMemoryVariable.Value.GetType())
                    {
                        return new List<string> { "Parameter type does not match" };
                    }
                    else
                    {
                        value = listItemValue;
                    }
                }

                if (i == indexFrom)
                {
                    instancedList.Add(value);
                    if (indexTo == null)
                    {
                        break;
                    }
                    continue;
                }

                if (i > indexTo)
                {
                    break;
                }

                instancedList.Add(value);
            }

            instanceProperty.SetValue(CommandInstance, instancedList);
            return new List<string>();
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
                if (instanceProperty.PropertyType != inMemoryVariable.Value.GetType())
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