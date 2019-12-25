using System;
using InteractiveConsole.Models.Storage;
using Microsoft.EntityFrameworkCore;

namespace InteractiveConsole.Storage.Persistent
{
    public class PersistentStorageDbContext : DbContext
    {
        public PersistentStorageDbContext() { }
        public PersistentStorageDbContext(DbContextOptions<PersistentStorageDbContext> dbContextOptions)
            : base(dbContextOptions) { }

        public DbSet<PersistentStorageVariable> Variables { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data Source={AppContext.BaseDirectory}/Variables.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            PersistentStorageVariable.BuildModel(modelBuilder);
        }
    }
}