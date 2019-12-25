using System.Text;
using System.Collections.Generic;
using System.Linq;
using Utf8Json;
using InteractiveConsole.Models.Storage;

namespace InteractiveConsole.Storage.Persistent
{
    public class PersistentVariableStorage : IPersistentVariableStorage
    {
        public bool Save(InMemoryStorageVariable variable)
        {
            using (var ctx = new PersistentStorageDbContext())
            {
                var existingVariable = ctx.Variables.FirstOrDefault(x => x.Id == variable.Id);
                if (existingVariable != null)
                {
                    ctx.Variables.Remove(existingVariable);
                }
                try
                {
                    var variableSerialized = JsonSerializer.Serialize(variable);
                    ctx.Variables.Add(new PersistentStorageVariable
                    {
                        Id = variable.Id,
                        Value = Encoding.UTF8.GetString(variableSerialized)
                    });
                    ctx.SaveChanges();
                    return true;
                }
                catch
                {
                    // Ignored
                }
            }

            return false;
        }

        public void Delete(int id)
        {
            using var ctx = new PersistentStorageDbContext();
            var variable = ctx.Variables.FirstOrDefault(x => x.Id == id);
            if (variable != null)
            {
                ctx.Variables.Remove(variable);
                ctx.SaveChanges();
            }
        }

        public List<InMemoryStorageVariable> Get()
        {
            using var ctx = new PersistentStorageDbContext();
            var variables = new List<InMemoryStorageVariable>();

            foreach (var variable in ctx.Variables)
            {
                var deserializedVariable = JsonSerializer.Deserialize<InMemoryStorageVariable>(variable.Value);
                variables.Add(deserializedVariable);
            }

            return variables;
        }
    }
}