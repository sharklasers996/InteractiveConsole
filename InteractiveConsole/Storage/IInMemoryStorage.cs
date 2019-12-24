using System.Collections.Generic;

namespace InteractiveConsole.Storage
{
    public interface IInMemoryStorage
    {
        List<InMemoryStorageVariable> Variables { get; }
        InMemoryStorageVariable Add(object value, string producedBy);
        InMemoryStorageVariable Add(object value, ParameterParserResult parserResult);
        InMemoryStorageVariable TryGetVariable(string stringId);
    }
}