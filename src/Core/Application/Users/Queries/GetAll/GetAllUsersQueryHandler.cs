using Application.Common.Interfaces;
using Application.Common.Pagination;
using Application.Common.Pagination.Models;
using Application.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;

namespace Application.Users.Queries.GetAll
{
    public class GetAllUsersQueryHandler(
        ICurrentUserService currentUserService,
        IManagersService managersService,
        IMapper mapper) : IRequestHandler<GetAllUsersQuery, PaginationResultViewModel<GetAllUsersViewModel>>
    {
        public async Task<PaginationResultViewModel<GetAllUsersViewModel>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            if (!request.IsValid(out string errorMessage))
                throw new BadRequestException(errorMessage);

            IQueryable<GetAllUsersViewModel> query = managersService.GetUsers()
                .Where(x => x.Id != currentUserService.UserId)
                .Where(x => string.IsNullOrEmpty(request.Term) ||
                    x.FirstName.Contains(request.Term) ||
                    x.LastName.Contains(request.Term) ||
                    (x.UserName != null && x.UserName.Contains(request.Term)))
                .ProjectTo<GetAllUsersViewModel>(mapper.ConfigurationProvider)
                .OrderBy(x => x.UserName ?? string.Empty);

            PaginatedList<GetAllUsersViewModel> paginatedList = await PaginatedList<GetAllUsersViewModel>.CreateAsync(
                query,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            return new PaginationResultViewModel<GetAllUsersViewModel>(
                paginatedList,
                paginatedList.PageNumber,
                paginatedList.TotalCount,
                paginatedList.TotalPages);
        }
    }
}