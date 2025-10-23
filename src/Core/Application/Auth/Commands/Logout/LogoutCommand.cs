using Application.Common.Security;
using MediatR;

namespace Application.Auth.Commands.Logout
{
    [Authorize(Policy = "RequireAuthorization")]
    public record LogoutCommand(string RefreshToken) : IRequest;
}