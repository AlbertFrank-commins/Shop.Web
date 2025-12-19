using System;
using System.Linq;
using Shop.Web.Data;
using Shop.Web.Entities.Identity;
using Shop.Web.Services.Identity;

namespace Shop.Web.Data
{
    
    public static class DbSeeder
    {
        public static void Seed(ShopDbContext db)
        {
            
            if (db.Users.Any())
                return;

            var password = "Admin123*";

            PasswordHasher.CreateHash(
                password,
                out var passwordHash,
                out var passwordSalt
            );

            var admin = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@shop.com",
                FullName = "Admin Shop",
                Role = "ADMIN",
                IsActive = true,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedAt = DateTime.UtcNow
            };

            db.Users.Add(admin);
            db.SaveChanges();
        }
    }
}
