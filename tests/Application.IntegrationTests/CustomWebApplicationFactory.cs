using Application.Common.Interfaces;
using Application.IntegrationTests.Wrappers;
using Domain.Entities.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace Application.IntegrationTests
{
    using static Testing;

    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(configurationBuilder =>
            {
                IConfigurationRoot integrationConfig = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();

                configurationBuilder.AddConfiguration(integrationConfig);
            });

            builder.ConfigureServices((builder, services) =>
            {
                services.Remove<ICurrentUserService>()
                    .AddTransient(provider => Mock.Of<ICurrentUserService>(s => s.UserId == GetCurrentUserId()));

                services.Remove<DbContextOptions<ApplicationDbContext>>()
                    .AddDbContext<ApplicationDbContext>((sp, options) =>
                        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                            builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

                services.Remove<IManagersService>();
                services.AddScoped<IManagersService>(sp =>
                {
                    UserManager<AppUser> userManager = sp.GetRequiredService<UserManager<AppUser>>();
                    IAuthorizationService authorizationService = sp.GetRequiredService<IAuthorizationService>();
                    IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory = sp.GetRequiredService<IUserClaimsPrincipalFactory<AppUser>>();
                    SignInManager<AppUser> signInManager = sp.GetRequiredService<SignInManager<AppUser>>();

                    return new ManagersServiceWrapper(
                        authorizationService,
                        userClaimsPrincipalFactory,
                        userManager,
                        signInManager);
                });
            });
        }
    }
}