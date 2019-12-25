using Microsoft.EntityFrameworkCore;

namespace InteractiveConsole.Models.Storage
{
    public class PersistentStorageVariable
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PersistentStorageVariable>()
                .ToTable("Storage")
                .HasKey(x => x.Id);
        }
    }
}