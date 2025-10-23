using Application.Common.Interfaces;

namespace Application.Auth
{
    public static class TokenHelper
    {
        public static TokenResponse GenerateJwt(
            string userId,
            string[] roles,
            IJwtFactory jwtFactory) => new(
                AuthToken: jwtFactory.GenerateEncodedToken(userId, roles),
                RefreshToken: jwtFactory.GenerateEncodedToken()
        );
    }
}