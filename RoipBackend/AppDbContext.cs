using Microsoft.EntityFrameworkCore;
using RoipBackend.Models;

namespace RoipBackend
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserConnection> UserConnection { get; set; }
        public DbSet<Logger> Logger { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add RowVersion property to all entities
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.Entity(entityType.ClrType).Property<byte[]>("RowVersion").IsRowVersion();
            }
        }
    }
}
