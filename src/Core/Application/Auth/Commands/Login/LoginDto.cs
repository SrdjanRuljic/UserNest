namespace Application.Auth.Commands.Login
{
    public record LoginDto(
        string AuthToken,
        string RefreshToken);
}