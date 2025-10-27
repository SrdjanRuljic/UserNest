using Application.Common.Security;
using Domain.Enums;
using MediatR;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Application.Users.Commands.Insert
{
    [Authorize(Policy = nameof(Policies.RequireAdminRole))]
    [SwaggerSchema("User creation command with all required fields")]
    public class InsertUserCommand : IRequest<string>
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [SwaggerSchema("User's unique username")]
        public string UserName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [SwaggerSchema("User's email address")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        [SwaggerSchema("User's password")]
        public string Password { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
        [SwaggerSchema("User's first name")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
        [SwaggerSchema("User's last name")]
        public string LastName { get; set; } = string.Empty;
        
        [Range(1, long.MaxValue, ErrorMessage = "Language ID must be a positive number")]
        [SwaggerSchema("User's preferred language ID")]
        public long? LanguageId { get; set; }
        
        [Required(ErrorMessage = "Role is required")]
        [StringLength(50, ErrorMessage = "Role cannot exceed 50 characters")]
        [SwaggerSchema("User's assigned role")]
        public string Role { get; set; } = string.Empty;
        
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [SwaggerSchema("User's phone number (optional)")]
        public string? PhoneNumber { get; set; }
    }
}