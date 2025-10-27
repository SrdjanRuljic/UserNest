using Application.Common.Security;
using Domain.Enums;
using MediatR;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Application.Users.Commands.Delete
{
    [Authorize(Policy = nameof(Policies.RequireAdminRole))]
    [SwaggerSchema("User deletion command")]
    public class DeleteUserCommand : IRequest
    {
        [Required(ErrorMessage = "User ID is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "User ID must be between 1 and 50 characters")]
        [SwaggerSchema("User's unique identifier")]
        public string Id { get; set; } = string.Empty;
    }
}
