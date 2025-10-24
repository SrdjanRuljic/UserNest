using Application.Common.Security;
using Domain.Enums;
using MediatR;

namespace Application.Users.Queries.GetById
{
    [Authorize(Policy = nameof(Policies.RequireAdminRole))]
    public record GetUserByIdQuery(string Id) : IRequest<GetUserByIdDto>;
}