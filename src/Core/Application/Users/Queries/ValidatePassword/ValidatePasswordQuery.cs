using Application.Common.Security;
using Domain.Enums;
using MediatR;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Application.Users.Queries.ValidatePassword
{
    [Authorize(Policy = nameof(Policies.RequireAuthorization))]
    [SwaggerSchema("Query to validate user password")]
    public class ValidatePasswordQuery : IRequest<bool>
    {
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        [SwaggerSchema("Password to validate")]
        public string Password { get; set; } = string.Empty;
    }
}