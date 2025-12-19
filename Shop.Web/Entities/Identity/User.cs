using System;

namespace Shop.Web.Entities.Identity
{
    public class User
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

        
        public DateTime CreatedAt { get; set; }
    }
}

