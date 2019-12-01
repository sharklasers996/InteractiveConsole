using System.Linq;
using System.Collections.Generic;

namespace InteractiveConsole.Storage
{
    public class InMemoryStorage : IInMemoryStorage
    {
        public List<InMemoryStorageVariable> Variables { get; } = new List<InMemoryStorageVariable>();

        public InMemoryStorage()
        {

        }

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
    }
}