using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.ViewModels.Users
{
    [SwaggerSchema("User summary view model for list view")]
    public class GetAllUsersViewModel
    {
        [SwaggerSchema("User's unique identifier")]
        public string Id { get; set; } = string.Empty;
        
        [SwaggerSchema("User's full name")]
        public string FullName { get; set; } = string.Empty;
        
        [SwaggerSchema("User's username")]
        public string UserName { get; set; } = string.Empty;
        
        [SwaggerSchema("User's email address")]
        public string Email { get; set; } = string.Empty;
        
        [SwaggerSchema("User's phone number")]
        public string? PhoneNumber { get; set; }
        
        [SwaggerSchema("User's role")]
        public string Role { get; set; } = string.Empty;
    }
}
