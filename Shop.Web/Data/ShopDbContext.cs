using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Shop.Web.Entities.Identity;


namespace Shop.Web.Data
{
    public class ShopDbContext : DbContext
    {
        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options) { }
        public DbSet<User> Users => Set<User>();
      

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Email).IsUnique();

                e.Property(x => x.Email).HasMaxLength(200).IsRequired();
                e.Property(x => x.FullName).HasMaxLength(200).IsRequired();
                e.Property(x => x.Role).HasMaxLength(30).IsRequired();

                e.Property(x => x.PasswordHash).IsRequired();
                e.Property(x => x.PasswordSalt).IsRequired();
            });
        }
    }
}
