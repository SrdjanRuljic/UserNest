using MediatR;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Application.Auth.Commands.Refresh
{
    [SwaggerSchema("Token refresh command")]
    public class RefreshTokenCommand : IRequest<RefreshViewModel>
    {
        [Required(ErrorMessage = "Refresh token is required")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Refresh token must be between 10 and 500 characters")]
        [SwaggerSchema("JWT refresh token")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}