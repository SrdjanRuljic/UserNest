using Application.Common.Pagination.Models;
using Application.Common.Security;
using Domain.Enums;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Users.Queries.GetAll
{
    [Authorize(Policy = nameof(Policies.RequireAdminRole))]
    public class GetAllUsersQuery : PaginationViewModel, IRequest<PaginationResultDto<GetAllUsersDto>>
    {
        [StringLength(100, ErrorMessage = "Search term cannot exceed 100 characters")]
        public string? Term { get; set; }
        
        public GetAllUsersQuery() : base()
        {
        }
        
        public GetAllUsersQuery(int pageNumber, int pageSize, string? term = null) : base(pageNumber, pageSize)
        {
            Term = term;
        }
    }
}