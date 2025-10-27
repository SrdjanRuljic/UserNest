using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Auth.Commands.Refresh
{
    public class RefreshTokenCommand : IRequest<RefreshDto>
    {
        [Required(ErrorMessage = "Refresh token is required")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Refresh token must be between 10 and 500 characters")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}