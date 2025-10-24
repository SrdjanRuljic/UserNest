using Application.Common.Pagination.Models;
using Application.Common.Security;
using Domain.Enums;
using MediatR;

namespace Application.Users.Queries.GetAll
{
    [Authorize(Policy = nameof(Policies.RequireAdminRole))]
    public record GetAllUsersQuery(
        int PageNumber = 1,
        int PageSize = 10,
        string? Term = null) : PaginationViewModel(PageNumber, PageSize), IRequest<PaginationResultViewModel<GetAllUsersViewModel>>;
}