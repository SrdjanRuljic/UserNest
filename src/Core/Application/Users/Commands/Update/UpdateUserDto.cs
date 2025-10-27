using System.ComponentModel.DataAnnotations;

namespace Application.Users.Commands.Update
{
    public class UpdateUserDto
    {
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string? FirstName { get; set; }
        
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public string? LastName { get; set; }
        
        [StringLength(256, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 256 characters")]
        public string? UserName { get; set; }
        
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
        public string? Email { get; set; }
        
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string? Password { get; set; }
        
        [Range(1, long.MaxValue, ErrorMessage = "Language ID must be a positive number")]
        public long? LanguageId { get; set; }
    }
}
