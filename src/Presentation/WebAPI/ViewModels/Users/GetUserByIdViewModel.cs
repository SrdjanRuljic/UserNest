using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.ViewModels.Users
{
    [SwaggerSchema("User details view model")]
    public class GetUserByIdViewModel
    {
        [SwaggerSchema("User's email address")]
        public string Email { get; set; } = string.Empty;
        
        [SwaggerSchema("User's first name")]
        public string FirstName { get; set; } = string.Empty;
        
        [SwaggerSchema("User's full name")]
        public string FullName { get; set; } = string.Empty;
        
        [SwaggerSchema("User's unique identifier")]
        public string Id { get; set; } = string.Empty;
        
        [SwaggerSchema("Whether the user is deleted")]
        public bool IsDeleted { get; set; }
        
        [SwaggerSchema("User's last name")]
        public string LastName { get; set; } = string.Empty;
        
        [SwaggerSchema("User's phone number")]
        public string? PhoneNumber { get; set; }
        
        [SwaggerSchema("User's role")]
        public string Role { get; set; } = string.Empty;
        
        [SwaggerSchema("User's username")]
        public string UserName { get; set; } = string.Empty;
        
        [SwaggerSchema("User's preferred language")]
        public string? Language { get; set; }
        
        [SwaggerSchema("User's culture")]
        public string? Culture { get; set; }
    }
}
