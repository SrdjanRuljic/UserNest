using AutoMapper;
using Application.Auth.Commands.Login;
using Application.Auth.Commands.Refresh;
using Application.Common.Pagination.Models;
using Application.Users.Queries.GetAll;
using Application.Users.Queries.GetById;
using WebAPI.ViewModels.Auth;
using WebAPI.ViewModels.Common;
using WebAPI.ViewModels.Users;

namespace WebAPI.Mappings
{
    public class ViewModelMappingProfile : Profile
    {
        public ViewModelMappingProfile()
        {
            // Auth output mappings
            CreateMap<LoginDto, LoginViewModel>();
            CreateMap<RefreshDto, RefreshViewModel>();

            // User output mappings
            CreateMap<GetAllUsersDto, GetAllUsersViewModel>();
            CreateMap<GetUserByIdDto, GetUserByIdViewModel>();

            // Pagination mappings
            CreateMap<PaginationResultDto<GetAllUsersDto>, PaginationResultViewModel<GetAllUsersViewModel>>()
                .ForMember(dest => dest.List, opt => opt.MapFrom(src => src.List));
        }
    }
}
