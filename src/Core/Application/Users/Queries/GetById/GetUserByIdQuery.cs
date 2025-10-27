using Application.Common.Security;
using Domain.Enums;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Users.Queries.GetById
{
    [Authorize(Policy = nameof(Policies.RequireAdminRole))]
    public class GetUserByIdQuery : IRequest<GetUserByIdDto>
    {
        [Required(ErrorMessage = "User ID is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "User ID must be between 1 and 50 characters")]
        public string Id { get; set; } = string.Empty;
    }
}