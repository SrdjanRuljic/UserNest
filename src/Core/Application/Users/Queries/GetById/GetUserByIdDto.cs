using Application.Common.Behaviors;
using Application.Mappings;
using AutoMapper;
using Domain.Entities.Identity;

namespace Application.Users.Queries.GetById
{
    public class GetUserByIdDto : IMapFrom<AppUser>
    {
        public string Email { get; set; } = string.Empty;
        
        public string FirstName { get; set; } = string.Empty;
        
        public string FullName { get; set; } = string.Empty;
        
        public string Id { get; set; } = string.Empty;
        
        public bool IsDeleted { get; set; }
        
        public string LastName { get; set; } = string.Empty;
        
        public string? PhoneNumber { get; set; }
        
        public string Role { get; set; } = string.Empty;
        
        public string UserName { get; set; } = string.Empty;
        
        public string? Language { get; set; }
        
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