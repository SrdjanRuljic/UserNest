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
        IMapper mapper) : IRequestHandler<GetAllUsersQuery, PaginationResultDto<GetAllUsersDto>>
    {
        public async Task<PaginationResultDto<GetAllUsersDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            if (!request.IsValid(out string errorMessage))
                throw new BadRequestException(errorMessage);

            IQueryable<GetAllUsersDto> query = managersService.GetUsers()
                .Where(x => x.Id != currentUserService.UserId)
                .Where(x => string.IsNullOrEmpty(request.Term) ||
                    x.FirstName.Contains(request.Term) ||
                    x.LastName.Contains(request.Term) ||
                    (x.UserName != null && x.UserName.Contains(request.Term)))
                .ProjectTo<GetAllUsersDto>(mapper.ConfigurationProvider)
                .OrderBy(x => x.UserName ?? string.Empty);

            PaginatedList<GetAllUsersDto> paginatedList = await PaginatedList<GetAllUsersDto>.CreateAsync(
                query,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            return new PaginationResultDto<GetAllUsersDto>(
                paginatedList,
                paginatedList.PageNumber,
                paginatedList.TotalCount,
                paginatedList.TotalPages);
        }
    }
}