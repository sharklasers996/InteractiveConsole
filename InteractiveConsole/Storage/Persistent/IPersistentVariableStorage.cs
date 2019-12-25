using System.Collections.Generic;
using InteractiveConsole.Models.Storage;

namespace InteractiveConsole.Storage.Persistent
{
    public interface IPersistentVariableStorage
    {
        void Delete(int id);
        List<InMemoryStorageVariable> Get();
        bool Save(InMemoryStorageVariable variable);
    }
}