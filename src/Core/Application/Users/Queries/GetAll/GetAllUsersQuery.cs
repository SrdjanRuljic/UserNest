using Application.Common.Pagination.Models;
using Application.Common.Security;
using Domain.Enums;
using MediatR;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Application.Users.Queries.GetAll
{
    [Authorize(Policy = nameof(Policies.RequireAdminRole))]
    [SwaggerSchema("Query to get all users with pagination and optional search")]
    public class GetAllUsersQuery : PaginationViewModel, IRequest<PaginationResultViewModel<GetAllUsersViewModel>>
    {
        [StringLength(100, ErrorMessage = "Search term cannot exceed 100 characters")]
        [SwaggerSchema("Optional search term to filter users by name or email")]
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