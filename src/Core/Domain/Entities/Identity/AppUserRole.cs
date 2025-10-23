using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Identity
{
    public class AppUserRole : IdentityUserRole<string>
    {
        public AppRole Role { get; set; } = null!;
        public AppUser User { get; set; } = null!;
    }
}