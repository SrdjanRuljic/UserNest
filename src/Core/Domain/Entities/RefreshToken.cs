using Domain.Entities.Identity;

namespace Domain.Entities
{
    public class RefreshToken
    {
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;

        #region [Entities]

        public AppUser User { get; set; } = null!;

        #endregion [Entities]

        public RefreshToken(string userId, string token)
        {
            UserId = userId;
            Token = token;
        }

        public RefreshToken()
        {
        }
    }
}