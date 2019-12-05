using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections;
using InteractiveConsole.Extensions;

namespace InteractiveConsole.Storage
{
    public class InMemoryStorage : IInMemoryStorage
    {
        public List<InMemoryStorageVariable> Variables { get; } = new List<InMemoryStorageVariable>();

        public void Add(object value, ParameterParserResult parserResult)
        {
            var variable = new InMemoryStorageVariable
            {
                Id = Variables.Any() ? Variables.Max(x => x.Id) + 1 : 1,
                Value = value,
                ProducedByCommand = parserResult.CommandName
            };

            var valueType = value.GetType();
            if (valueType.IsGenericType 
                && valueType.GetGenericTypeDefinition() == typeof(List<>))
            {
                variable.IsList = true;
                variable.Length = (value as IList).Count;
            }

            if (valueType.IsNumericType())
            {
                variable.IsNumber = true;
            }

            if (value is string _)
            {
                variable.IsString = true;
            }

            Variables.Add(variable);
        }

        public InMemoryStorageVariable TryGetVariable(string stringId)
        {
            var variableMatch = Regex.Match(stringId, @"#(?<varId>\d+)");
            if (variableMatch.Success)
            {
                if (int.TryParse(variableMatch.Groups["varId"].ToString(), out var id))
                {
                    return Variables.FirstOrDefault(x => x.Id == id);
                }
            }

            return null;
        }
    }
}