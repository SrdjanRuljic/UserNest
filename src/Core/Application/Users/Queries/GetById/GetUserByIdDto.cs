using Application.Common.Behaviors;
using Application.Mappings;
using AutoMapper;
using Domain.Entities.Identity;
using Swashbuckle.AspNetCore.Annotations;

namespace Application.Users.Queries.GetById
{
    [SwaggerSchema("User details response")]
    public class GetUserByIdDto : IMapFrom<AppUser>
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

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AppUser, GetUserByIdDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => FullNameResolver.Resolve(src)))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.UserRoles.Select(x => x.Role.Name).First()))
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language!.Name))
                .ForMember(dest => dest.Culture, opt => opt.MapFrom(src => src.Language!.Culture));
        }
    }
}