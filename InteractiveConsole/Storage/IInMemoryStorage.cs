using System.Collections.Generic;
using InteractiveConsole.Models.Storage;

namespace InteractiveConsole.Storage
{
    public interface IInMemoryStorage
    {
        List<InMemoryStorageVariable> Variables { get; }
        InMemoryStorageVariable Add(object value, string producedBy);
        InMemoryStorageVariable Add(object value, ParameterParserResult parserResult);
        InMemoryStorageVariable TryGetVariable(string stringId);
        bool DeleteVariable(int id);
    }
}