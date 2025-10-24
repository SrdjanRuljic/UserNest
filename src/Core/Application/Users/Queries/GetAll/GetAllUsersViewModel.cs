using Application.Common.Behaviors;
using Application.Mappings;
using AutoMapper;
using Domain.Entities.Identity;

namespace Application.Users.Queries.GetAll
{
    public class GetAllUsersViewModel : IMapFrom<AppUser>
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = string.Empty;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AppUser, GetAllUsersViewModel>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => FullNameResolver.Resolve(src)))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.UserRoles.Select(x => x.Role.Name).First()));
        }
    }
}