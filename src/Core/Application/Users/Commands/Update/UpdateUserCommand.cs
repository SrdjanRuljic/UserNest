using Application.Common.Security;
using Domain.Enums;
using MediatR;

namespace Application.Users.Commands.Update
{
    [Authorize(Policy = nameof(Policies.RequireAdminRole))]
    public record UpdateUserCommand(
        string Id,
        string? FirstName = null,
        string? LastName = null,
        string? UserName = null,
        string? Email = null,
        string? Password = null,
        long? LanguageId = null) : IRequest<string>;
}