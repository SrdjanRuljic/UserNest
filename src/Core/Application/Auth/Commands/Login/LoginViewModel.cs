using Swashbuckle.AspNetCore.Annotations;

namespace Application.Auth.Commands.Login
{
    [SwaggerSchema("Login response containing authentication tokens")]
    public record LoginViewModel(
        [SwaggerSchema("JWT access token")]
        string AuthToken,
        [SwaggerSchema("JWT refresh token")]
        string RefreshToken);
}