namespace Application.Auth.Commands.Refresh
{
    public record RefreshDto(
        string AuthToken,
        string RefreshToken);
}
