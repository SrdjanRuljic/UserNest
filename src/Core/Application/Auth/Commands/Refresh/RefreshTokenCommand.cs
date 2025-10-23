using MediatR;

namespace Application.Auth.Commands.Refresh
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<RefreshViewModel>;
}