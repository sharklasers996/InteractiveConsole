using System.Reflection;
using System.Linq;
using System;
using InteractiveConsole.Models;
using InteractiveConsole.Extensions;
using InteractiveConsole.Storage;
using System.Collections.Generic;
using System.Collections;
using InteractiveConsole.Models.Storage;

namespace InteractiveConsole
{
    public class ParameterProcessor
    {
        private const string TypeDoesNotMatchErrorMessage = "Parameter type does not match";
        private const string IndexOutOfBoundsErrorMessage = "Index is out of bounds";

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
                var error = string.Empty;
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
                    if (parameter.IndexFrom == null
                        && parameter.IndexTo != null)
                    {
                        return "Starting index is required";
                    }

                    if (parameter.IndexFrom != null
                        && parameter.IndexTo == null)
                    {
                        error = SetInMemoryVariableValueAtIndex(instanceProperty, inMemoryVariable, (int)parameter.IndexFrom);
                    }
                    else if (parameter.IndexFrom != null
                        && parameter.IndexTo != null)
                    {
                        error = SetInMemoryVariableValueInRange(instanceProperty, inMemoryVariable, (int)parameter.IndexFrom, (int)parameter.IndexTo);
                    }
                    else
                    {
                        error = SetInMemoryVariableValue(instanceProperty, inMemoryVariable);
                    }
                }
                else
                {
                    error = SetParameterValue(instanceProperty, parameter);
                }

                if (!String.IsNullOrEmpty(error))
                {
                    return error;
                }
            }

            return string.Empty;
        }

        private string SetInMemoryVariableValueAtIndex(PropertyInfo instanceProperty, InMemoryStorageVariable inMemoryVariable, int index)
        {
            if (index < 0)
            {
                return IndexOutOfBoundsErrorMessage;
            }

            int listLength = (int)inMemoryVariable.TypeInfo.Type.GetProperty("Count").GetValue(inMemoryVariable.Value, null);
            if (index >= listLength)
            {
                return IndexOutOfBoundsErrorMessage;
            }

            var propertyTypeInfo = instanceProperty.PropertyType.ToTypeInfo();
            if (propertyTypeInfo.IsList
                && !inMemoryVariable.TypeInfo.IsList
                && !inMemoryVariable.TypeInfo.EqualsListType(propertyTypeInfo))
            {
                return TypeDoesNotMatchErrorMessage;
            }
            else if (propertyTypeInfo.IsList
              && inMemoryVariable.TypeInfo.IsList
              && !inMemoryVariable.TypeInfo.Equals(propertyTypeInfo))
            {
                return TypeDoesNotMatchErrorMessage;
            }
            else if (!propertyTypeInfo.IsList
              && !inMemoryVariable.TypeInfo.IsList
              && !inMemoryVariable.TypeInfo.Equals(propertyTypeInfo))
            {
                return TypeDoesNotMatchErrorMessage;
            }
            else if (!propertyTypeInfo.IsList
                && inMemoryVariable.TypeInfo.IsList
                && !propertyTypeInfo.EqualsListType(inMemoryVariable.TypeInfo))
            {
                return TypeDoesNotMatchErrorMessage;
            }

            var inMemoryVariableItemAtIndex = inMemoryVariable.TypeInfo.Type.GetProperty("Item").GetValue(inMemoryVariable.Value, new object[] { index });

            if (!propertyTypeInfo.IsList)
            {
                instanceProperty.SetValue(CommandInstance, inMemoryVariableItemAtIndex);
            }
            else
            {
                var instancedList = (IList)typeof(List<>)
                    .MakeGenericType(inMemoryVariable.TypeInfo.Type.GetListItemType())
                    .GetConstructor(Type.EmptyTypes)
                    .Invoke(null);

                instancedList.Add(inMemoryVariableItemAtIndex);
                instanceProperty.SetValue(CommandInstance, instancedList);
            }

            return string.Empty;
        }

        private string SetInMemoryVariableValueInRange(PropertyInfo instanceProperty, InMemoryStorageVariable inMemoryVariable, int indexFrom, int indexTo)
        {
            var propertyTypeInfo = instanceProperty.PropertyType.ToTypeInfo();
            if (!propertyTypeInfo.Equals(inMemoryVariable.TypeInfo))
            {
                return TypeDoesNotMatchErrorMessage;
            }
            int listLength = (int)inMemoryVariable.TypeInfo.Type.GetProperty("Count").GetValue(inMemoryVariable.Value, null);
            if (indexFrom < 0
                || indexFrom > listLength
                || indexTo >= listLength)
            {
                return IndexOutOfBoundsErrorMessage;
            }

            var instancedList = (IList)typeof(List<>)
                .MakeGenericType(inMemoryVariable.TypeInfo.Type.GetListItemType())
                .GetConstructor(Type.EmptyTypes)
                .Invoke(null);

            for (var i = 0; i < listLength; i++)
            {
                if (i < indexFrom)
                {
                    continue;
                }

                if (i >= indexTo)
                {
                    break;
                }

                var listItemValue = inMemoryVariable.TypeInfo.Type.GetProperty("Item").GetValue(inMemoryVariable.Value, new object[] { i });
                instancedList.Add(listItemValue);
            }

            instanceProperty.SetValue(CommandInstance, instancedList);

            return string.Empty;
        }

        private string SetInMemoryVariableValue(PropertyInfo instanceProperty, InMemoryStorageVariable inMemoryVariable)
        {
            var propertyTypeInfo = instanceProperty.PropertyType.ToTypeInfo();

            if (propertyTypeInfo.IsNumber
                && inMemoryVariable.TypeInfo.IsNumber)
            {
                instanceProperty.SetValue(CommandInstance, inMemoryVariable.Value.ToString());
            }
            else if (propertyTypeInfo.Type == typeof(InMemoryStorageVariable))
            {
                instanceProperty.SetValue(CommandInstance, inMemoryVariable);
            }
            else
            {
                if (!propertyTypeInfo.Equals(inMemoryVariable.TypeInfo))
                {
                    return TypeDoesNotMatchErrorMessage;
                }

                instanceProperty.SetValue(CommandInstance, inMemoryVariable.Value);
            }

            return string.Empty;
        }

        private string SetParameterValue(PropertyInfo instanceProperty, Parameter parameter)
        {
            object convertedParameterValue = null;
            var propertyTypeInfo = instanceProperty.PropertyType.ToTypeInfo();

            if (propertyTypeInfo.IsNumber
                && int.TryParse(parameter.Value, out var numberParameter))
            {
                convertedParameterValue = numberParameter;
            }

            if (propertyTypeInfo.IsEnum
                && Enum.TryParse(propertyTypeInfo.Type, parameter.Value.ToString(), out var parsedEnum))
            {
                convertedParameterValue = parsedEnum;
            }

            if (propertyTypeInfo.IsBool
                && bool.TryParse(parameter.Value.ToString(), out var parsedBool))
            {
                convertedParameterValue = parsedBool;
            }

            if (convertedParameterValue == null)
            {
                convertedParameterValue = parameter.Value;
            }

            if (!propertyTypeInfo.Equals(convertedParameterValue.ToTypeInfo()))
            {
                return TypeDoesNotMatchErrorMessage;
            }
            else
            {
                instanceProperty.SetValue(CommandInstance, convertedParameterValue);
            }

            return string.Empty;
        }
    }
}