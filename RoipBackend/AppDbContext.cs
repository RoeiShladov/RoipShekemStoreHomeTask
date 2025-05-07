using Microsoft.EntityFrameworkCore;
using RoipBackend.Models;

namespace RoipBackend
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        public DbSet<User> Users { get; set; }
        
        public DbSet<Logger> Logger { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<User>()
            //   .Property(u => u.RowVersion)
            //   .HasDefaultValue(new byte[8]); // Default value for RowVersion

            //modelBuilder.Entity<Product>()
            //    .Property(p => p.RowVersion)
            //    .HasDefaultValue(new byte[8]); // Default value for RowVersion


            //Add RowVersion property to all entities
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.Entity(entityType.ClrType).Property<byte[]>("RowVersion").IsRowVersion().IsRequired(false);
            }
        }

        //Usage in the future only if the system will need to store the connection of the users in the database
        //public DbSet<UserConnection> UserConnection { get; set; }
    }
}
