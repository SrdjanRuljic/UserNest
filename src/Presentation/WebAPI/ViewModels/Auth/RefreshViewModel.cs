using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.ViewModels.Auth
{
    [SwaggerSchema("Token refresh response view model")]
    public record RefreshViewModel(
        [SwaggerSchema("New JWT access token")]
        string AuthToken,
        [SwaggerSchema("New JWT refresh token")]
        string RefreshToken);
}
