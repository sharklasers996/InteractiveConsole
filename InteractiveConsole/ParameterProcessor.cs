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
    // TODO: clean up
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

        public string SetParameters()
        {
            var commandType = CommandInstance.GetType();
            foreach (var option in CommandInfo.Options.OrderBy(x => x.Required))
            {
                var parameter = ParserResult.Parameters?.FirstOrDefault(x => x.Name == option.Name);
                if (parameter == null)
                {
                    if (option.Required)
                    {
                        return $"Option '{option.Name}' is required";
                    }

                    continue;
                }

                var instanceProperty = commandType.GetProperty(parameter.Name);
                var inMemoryVariable = _InMemoryStorage.TryGetVariable(parameter.Value);
                if (inMemoryVariable != null)
                {
                    if (parameter.IndexFrom != null)
                    {
                        var error = SetInMemoryVariableIndexValue(instanceProperty, inMemoryVariable, (int)parameter.IndexFrom, parameter.IndexTo);
                        if (!String.IsNullOrEmpty(error))
                        {
                            return error;
                        }
                    }
                    else
                    {
                        var error = SetInMemoryVariableValue(instanceProperty, inMemoryVariable);
                        if (!String.IsNullOrEmpty(error))
                        {
                            return error;
                        }
                    }
                }
                else
                {
                    var error = SetParameterValue(instanceProperty, parameter);
                    if (!String.IsNullOrEmpty(error))
                    {
                        return error;
                    }
                }
            }

            return string.Empty;
        }

        private string SetInMemoryVariableIndexValue(PropertyInfo instanceProperty, InMemoryStorageVariable inMemoryVariable, int indexFrom, int? indexTo)
        {
            if (!inMemoryVariable.IsList)
            {
                return "Index can only be used on list";
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
                return "Index is out of bounds";
            }

            if (indexTo != null
                && indexTo >= listLength)
            {
                return "Index is out of bounds";
            }

            var addedValues = new List<object>();
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
                    if (instanceProperty.PropertyType.IsList())
                    {
                        if (instanceProperty.PropertyType.GetListItemType() != typeof(object)
                            && instanceProperty.PropertyType.GetListItemType() != listItemValueType)
                        {
                            return "Parameter type does not match";
                        }
                        else
                        {
                            value = listItemValue;
                        }
                    }
                    else
                    {
                        if (instanceProperty.PropertyType != typeof(object)
                            && instanceProperty.PropertyType != listItemValueType)
                        {
                            return "Parameter type does not match";
                        }
                        else
                        {
                            value = listItemValue;
                        }
                    }
                }

                if (i == indexFrom)
                {
                    instancedList.Add(value);
                    addedValues.Add(value);
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
                addedValues.Add(value);
            }

            if (indexTo == null
                && instancedList.Count == 1)
            {
                instanceProperty.SetValue(CommandInstance, addedValues.FirstOrDefault());
            }
            else
            {
                instanceProperty.SetValue(CommandInstance, instancedList);
            }

            return string.Empty;
        }

        private string SetInMemoryVariableValue(PropertyInfo instanceProperty, InMemoryStorageVariable inMemoryVariable)
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
                if (instanceProperty.PropertyType != typeof(object)
                    && instanceProperty.PropertyType != inMemoryVariable.Value.GetType())
                {
                    return "Parameter type does not match";
                }
                else
                {
                    instanceProperty.SetValue(CommandInstance, inMemoryVariable.Value);
                }
            }

            return string.Empty;
        }

        private string SetParameterValue(PropertyInfo instanceProperty, Parameter parameter)
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
                return "Parameter type does not match";
            }
            else
            {
                instanceProperty.SetValue(CommandInstance, parameterValue);
            }

            return string.Empty;
        }
    }
}