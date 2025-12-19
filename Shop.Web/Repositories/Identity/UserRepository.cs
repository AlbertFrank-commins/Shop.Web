using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop.Web.Data;
using Shop.Web.Entities.Identity;

namespace Shop.Web.Repositories.Identity
{
    public class UserRepository : IUserRepository
    {
        private readonly ShopDbContext _db;

        public UserRepository(ShopDbContext db)
        {
            _db = db;
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            return _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
