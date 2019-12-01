using System.Collections.Generic;

namespace InteractiveConsole.Storage
{
    public interface IInMemoryStorage
    {
        List<InMemoryStorageVariable> Variables { get; }
        void Add(object value, ParameterParserResult parserResult);
    }
}