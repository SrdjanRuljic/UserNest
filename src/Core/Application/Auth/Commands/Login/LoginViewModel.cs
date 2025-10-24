namespace Application.Auth.Commands.Login
{
    public record LoginViewModel(
        string AuthToken,
        string RefreshToken);
}