using InteractiveConsole.Models;

namespace InteractiveConsole.Storage
{
    public class InMemoryStorageVariable
    {
        public int Id { get; set; }
        public object Value { get; set; }
        public string ValueString { get { return Value.ToString(); } }
        public string ProducedByCommand { get; set; }
        public int Length { get; set; }
        public TypeInfo TypeInfo { get; set; }

        public override string ToString()
        {
            return $"#{Id}";
        }
    }
}