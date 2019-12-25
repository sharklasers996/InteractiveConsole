using System.Collections.Generic;
using InteractiveConsole.Models.Storage;

namespace InteractiveConsole.Storage
{
    public interface IInMemoryStorage
    {
        List<InMemoryStorageVariable> Variables { get; }
        VariableStorageResult Add(object value, string producedBy);
        VariableStorageResult Add(object value, ParameterParserResult parserResult);
        InMemoryStorageVariable TryGetVariable(string stringId);
        bool DeleteVariable(int id);
    }
}