using Application.Common.Security;
using Domain.Enums;
using MediatR;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Application.Users.Commands.Update
{
    [Authorize(Policy = nameof(Policies.RequireAdminRole))]
    [SwaggerSchema("User update command with optional fields")]
    public class UpdateUserCommand : IRequest<string>
    {
        [Required(ErrorMessage = "User ID is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "User ID must be between 1 and 50 characters")]
        [SwaggerSchema("User's unique identifier")]
        public string Id { get; set; } = string.Empty;
        
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
        [SwaggerSchema("User's first name (optional)")]
        public string? FirstName { get; set; }
        
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
        [SwaggerSchema("User's last name (optional)")]
        public string? LastName { get; set; }
        
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [SwaggerSchema("User's username (optional)")]
        public string? UserName { get; set; }
        
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [SwaggerSchema("User's email address (optional)")]
        public string? Email { get; set; }
        
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        [SwaggerSchema("User's new password (optional)")]
        public string? Password { get; set; }
        
        [Range(1, long.MaxValue, ErrorMessage = "Language ID must be a positive number")]
        [SwaggerSchema("User's preferred language ID (optional)")]
        public long? LanguageId { get; set; }
    }
}