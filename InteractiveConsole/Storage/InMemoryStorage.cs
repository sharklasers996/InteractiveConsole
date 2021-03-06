using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using InteractiveConsole.Extensions;
using InteractiveConsole.Models.Storage;

namespace InteractiveConsole.Storage
{
    public class InMemoryStorage : IInMemoryStorage
    {
        public List<InMemoryStorageVariable> Variables { get; private set; }

        public InMemoryStorage()
        {
            Variables = new List<InMemoryStorageVariable>();
        }

        public InMemoryStorageVariable Add(object value, ParameterParserResult parserResult)
        {
            return Add(value, parserResult.CommandName);
        }

        public InMemoryStorageVariable Add(object value, string producedBy)
        {
            var variable = new InMemoryStorageVariable
            {
                Id = Variables.Any() ? Variables.Max(x => x.Id) + 1 : 1,
                Value = value,
                Description = $"returned by {producedBy}",
                TypeInfo = value.ToTypeInfo()
            };

            if (variable.TypeInfo.IsList)
            {
                variable.Length = (int)variable.TypeInfo.Type.GetProperty("Count").GetValue(value, null);
            }

            Variables.Add(variable);
            return variable;
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

        public bool DeleteVariable(int id)
        {
            var variable = Variables.FirstOrDefault(x => x.Id == id);
            if (variable == null)
            {
                return false;
            }

            Variables.Remove(variable);

            return true;
        }
    }
}