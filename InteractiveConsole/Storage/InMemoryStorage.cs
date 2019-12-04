using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
                Description = $"From {parserResult.CommandName}"
            };
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