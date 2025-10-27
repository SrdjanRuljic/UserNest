using Domain.Entities.Identity;

namespace Domain.Entities
{
    public class Language
    {
        public string Culture { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? FlagPath { get; set; }
        public long Id { get; set; }
        public bool IsDeleted { get; set; }
        public string Name { get; set; } = string.Empty;

        #region [Collections]

        public ICollection<AppUser> UsersUsingLanguage { get; set; } = null!;

        #endregion [Collections]
    }
}