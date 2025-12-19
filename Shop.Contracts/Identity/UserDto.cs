using System;

namespace Shop.Contracts.Identity
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "USER";
        public bool IsActive { get; set; }
    }
}
