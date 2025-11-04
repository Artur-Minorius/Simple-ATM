using Microsoft.EntityFrameworkCore;
using Simple_ATM.DomainLayer.Entities;

namespace Simple_ATM.Infrastructure.Data
{
    public class AtmContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Operation> Operations { get; set; }

        public AtmContext(DbContextOptions<AtmContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.CardNumber)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }

    }
}
