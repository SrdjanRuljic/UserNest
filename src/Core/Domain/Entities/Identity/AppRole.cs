using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Identity
{
    public class AppRole : IdentityRole
    {
        #region [Collections]

        public ICollection<AppUserRole> UserRoles { get; set; } = null!;

        #endregion [Collections]
    }
}