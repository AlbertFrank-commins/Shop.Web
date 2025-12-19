using System.Threading.Tasks;
using Shop.Web.Entities.Identity;

namespace Shop.Web.Repositories.Identity
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
    }
}
