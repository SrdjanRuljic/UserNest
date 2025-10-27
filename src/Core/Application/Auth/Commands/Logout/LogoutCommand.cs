using Application.Common.Security;
using MediatR;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Application.Auth.Commands.Logout
{
    [Authorize(Policy = "RequireAuthorization")]
    [SwaggerSchema("User logout command")]
    public class LogoutCommand : IRequest
    {
        [Required(ErrorMessage = "Refresh token is required")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Refresh token must be between 10 and 500 characters")]
        [SwaggerSchema("JWT refresh token")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}