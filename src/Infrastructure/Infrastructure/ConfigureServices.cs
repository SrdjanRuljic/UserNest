using Application.Common.Interfaces;
using Domain.Entities.Identity;
using Infrastructure.Auth;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISaveChangesInterceptor, AuditableEntitySaveChangesInterceptor>();
            services.AddScoped<ApplicationDbContextInitializer>();

            string? connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(connectionString,
                    builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            });

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddErrorDescriber<CustomIdentityErrorDescriber>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider);

            services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(2));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = configuration["Jwt:Issuer"],
                            ValidateAudience = true,
                            ValidAudience = configuration["Jwt:Audience"],
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:AccessTokenSecret"]!))
                        };
                    });

            services.AddAuthorizationBuilder()
                    .AddPolicy(Domain.Enums.Policies.RequireAdminRole.ToString(), policy =>
                        policy.RequireAuthenticatedUser().RequireRole(Domain.Enums.Roles.Admin.ToString()))
                    .AddPolicy(Domain.Enums.Policies.RequireRegularUserRole.ToString(), policy =>
                        policy.RequireAuthenticatedUser().RequireRole(Domain.Enums.Roles.RegularUser.ToString()))
                    .AddPolicy(Domain.Enums.Policies.RequireAuthorization.ToString(), policy =>
                        policy.RequireAuthenticatedUser());

            services.AddTransient<IJwtFactory, JwtFactory>();
            services.AddTransient<IDateTimeService, DateTimeService>();
            services.AddTransient<IManagersService, ManagersService>();

            return services;
        }
    }
}