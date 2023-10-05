using Microsoft.AspNetCore.Identity;

namespace Hotel.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public required string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
