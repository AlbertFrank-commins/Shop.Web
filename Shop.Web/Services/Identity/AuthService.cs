using System.Threading.Tasks;
using Shop.BuildingBlocks.Results;
using Shop.Contracts.Identity;
using Shop.Web.Repositories.Identity;

namespace Shop.Web.Services.Identity
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;

        public AuthService(IUserRepository users)
        {
            _users = users;
        }

        public async Task<Result<UserDto>> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return Result<UserDto>.Fail("Email y Password son requeridos.");

            var user = await _users.GetByEmailAsync(request.Email.Trim());
            if (user is null)
                return Result<UserDto>.Fail("Credenciales inválidas.");

            if (!user.IsActive)
                return Result<UserDto>.Fail("Usuario inactivo.");

            var ok = PasswordHasher.Verify(
                request.Password,
                user.PasswordHash,
                user.PasswordSalt);

            if (!ok)
                return Result<UserDto>.Fail("Credenciales inválidas.");

            var dto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive
            };

            return Result<UserDto>.Ok(dto);


        }
    }
}
