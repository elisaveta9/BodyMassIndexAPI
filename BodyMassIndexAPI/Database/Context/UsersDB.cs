using BodyMassIndexAPI.Database.Entityes;
using Microsoft.EntityFrameworkCore;

namespace BodyMassIndexAPI.Database.Context
{
    internal class UsersDB : DbContext
    {
        public DbSet<Person> People { get; set; }

        public DbSet<Details> Details { get; set; }

        public UsersDB() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=usersdb;Username=postgres;Password=student");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().HasKey(k => k.Id);
            modelBuilder.Entity<Details>().HasKey(k => k.Id);
        }
    }
}
