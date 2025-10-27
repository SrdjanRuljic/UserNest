using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.ViewModels.Auth
{
    [SwaggerSchema("Login response view model containing authentication tokens")]
    public record LoginViewModel(
        [SwaggerSchema("JWT access token")]
        string AuthToken,
        [SwaggerSchema("JWT refresh token")]
        string RefreshToken);
}
