using Application.Common.Security;
using Domain.Enums;
using MediatR;

namespace Application.Users.Commands.Delete
{
    [Authorize(Policy = nameof(Policies.RequireAdminRole))]
    public record DeleteUserCommand(string Id) : IRequest;
}
