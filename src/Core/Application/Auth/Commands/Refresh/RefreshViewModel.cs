namespace Application.Auth.Commands.Refresh
{
    public record RefreshViewModel(
        string AuthToken,
        string RefreshToken);
}