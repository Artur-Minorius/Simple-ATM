using Microsoft.EntityFrameworkCore;
using Simple_ATM.Models.ATM_Data;

namespace Simple_ATM.Models
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
