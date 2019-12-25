namespace InteractiveConsole.Models.Storage
{
    public class VariableStorageResult
    {
        public InMemoryStorageVariable Variable { get; set; }
        public bool Persisted { get; set; }
    }
}