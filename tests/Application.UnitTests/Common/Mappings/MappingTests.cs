using Application.Mappings;
using Application.Users.Queries.GetById;
using Application.Users.Queries.GetAll;
using Application.Auth.Commands.Login;
using Application.Auth.Commands.Refresh;
using Application.Common.Pagination.Models;
using WebAPI.ViewModels.Auth;
using WebAPI.ViewModels.Users;
using WebAPI.ViewModels.Common;
using WebAPI.Mappings;
using AutoMapper;
using Domain.Entities.Identity;
using Microsoft.Extensions.Logging;

namespace Application.UnitTests.Common.Mappings
{
    public sealed class MappingTests
    {
        private readonly IConfigurationProvider _configuration;
        private readonly IMapper _mapper;

        public MappingTests()
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            _configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
                cfg.AddProfile<ViewModelMappingProfile>();
            }, loggerFactory);

            _mapper = _configuration.CreateMapper();
        }

        [Test]
        public void ShouldHaveValidConfiguration()
        {
            _configuration.AssertConfigurationIsValid();
        }

        [Test]
        [TestCase(typeof(AppUser), typeof(GetUserByIdDto))]
        [TestCase(typeof(AppUser), typeof(GetAllUsersDto))]
        [TestCase(typeof(LoginDto), typeof(LoginViewModel))]
        [TestCase(typeof(RefreshDto), typeof(RefreshViewModel))]
        [TestCase(typeof(GetAllUsersDto), typeof(GetAllUsersViewModel))]
        [TestCase(typeof(GetUserByIdDto), typeof(GetUserByIdViewModel))]
        [TestCase(typeof(PaginationResultDto<GetAllUsersDto>), typeof(PaginationResultViewModel<GetAllUsersViewModel>))]
        public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
        {
            object instance = GetInstanceOf(source);

            _mapper.Map(instance, source, destination);
        }

        private object GetInstanceOf(Type type)
        {
            if (type.GetConstructor(Type.EmptyTypes) != null)
                return Activator.CreateInstance(type)!;

            return null!;
        }
    }
}