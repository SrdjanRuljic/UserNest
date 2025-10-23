namespace Application.Auth
{
    public record TokenResponse(
        string AuthToken,
        string RefreshToken);
}