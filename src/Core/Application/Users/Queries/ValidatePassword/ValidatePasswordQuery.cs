using Application.Common.Security;
using Domain.Enums;
using MediatR;

namespace Application.Users.Queries.ValidatePassword
{
    [Authorize(Policy = nameof(Policies.RequireAuthorization))]
    public record ValidatePasswordQuery(string Password) : IRequest<bool>;
}