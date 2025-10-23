using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        #region [Audit]

        public DateTime Created { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastModified { get; set; }
        public string? LastModifiedBy { get; set; }

        #endregion [Audit]

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public long? LanguageId { get; set; }

        #region [Entities]

        public Language? Language { get; set; }

        #endregion [Entities]

        #region [Collections]

        public ICollection<RefreshToken> RefreshTokens { get; set; } = null!;
        public ICollection<AppUserRole> UserRoles { get; set; } = null!;

        #endregion [Collections]
    }
}