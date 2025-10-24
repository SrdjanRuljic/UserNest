using Application.Common.Security;
using Domain.Enums;
using MediatR;

namespace Application.Users.Commands.Insert
{
    [Authorize(Policy = nameof(Policies.RequireAdminRole))]
    public record InsertUserCommand(
        string UserName,
        string Email,
        string Password,
        string FirstName,
        string LastName,
        long? LanguageId,
        string Role,
        string? PhoneNumber = null) : IRequest<string>;
}