using Application.Common.Security;
using Domain.Enums;
using MediatR;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Application.Users.Queries.GetById
{
    [Authorize(Policy = nameof(Policies.RequireAdminRole))]
    [SwaggerSchema("Query to get user by ID")]
    public class GetUserByIdQuery : IRequest<GetUserByIdDto>
    {
        [Required(ErrorMessage = "User ID is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "User ID must be between 1 and 50 characters")]
        [SwaggerSchema("User's unique identifier")]
        public string Id { get; set; } = string.Empty;
    }
}