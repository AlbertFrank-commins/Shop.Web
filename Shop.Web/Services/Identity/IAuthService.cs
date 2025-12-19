using System.Threading.Tasks;
using Shop.Contracts.Identity;
using Shop.BuildingBlocks.Results;

namespace Shop.Web.Services.Identity
{
    public interface IAuthService
    {
        Task<Result<UserDto>> LoginAsync(LoginRequest request);
    }
}
