using Application.Common.Behaviors;
using Application.Mappings;
using AutoMapper;
using Domain.Entities.Identity;
using Swashbuckle.AspNetCore.Annotations;

namespace Application.Users.Queries.GetAll
{
    [SwaggerSchema("User summary for list view")]
    public class GetAllUsersViewModel : IMapFrom<AppUser>
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

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AppUser, GetAllUsersViewModel>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => FullNameResolver.Resolve(src)))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.UserRoles.Select(x => x.Role.Name).First()));
        }
    }
}