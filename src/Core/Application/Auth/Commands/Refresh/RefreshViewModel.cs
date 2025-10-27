using Swashbuckle.AspNetCore.Annotations;

namespace Application.Auth.Commands.Refresh
{
    [SwaggerSchema("Token refresh response")]
    public record RefreshViewModel(
        [SwaggerSchema("New JWT access token")]
        string AuthToken,
        [SwaggerSchema("New JWT refresh token")]
        string RefreshToken);
}