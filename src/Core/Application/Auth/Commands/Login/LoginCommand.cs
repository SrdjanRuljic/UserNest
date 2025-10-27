using MediatR;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Application.Auth.Commands.Login
{
    [SwaggerSchema("User login credentials")]
    public class LoginCommand : IRequest<LoginViewModel>
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [SwaggerSchema("User's username or email")]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        [SwaggerSchema("User's password")]
        public string Password { get; set; } = string.Empty;
    }
}