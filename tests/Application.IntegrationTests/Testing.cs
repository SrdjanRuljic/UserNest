using Domain.Entities.Identity;
using Infrastructure.Identity.Extensions;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;

namespace Application.IntegrationTests
{
    public class Testing : IAsyncLifetime
    {
        private static Respawner _checkpoint = null!;
        private static IConfiguration _configuration = null!;
        private static IServiceScopeFactory _scopeFactory = null!;
        private static WebApplicationFactory<Program> _factory = null!;
        private static string _currentUserId = string.Empty;

        public async Task InitializeAsync()
        {
            if (_factory == null)
            {
                _factory = new CustomWebApplicationFactory();
                _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
                _configuration = _factory.Services.GetRequiredService<IConfiguration>();

                string connectionString = _configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("DefaultConnection connection string not found.");

                _checkpoint = await Respawner.CreateAsync(connectionString, new RespawnerOptions
                {
                    DbAdapter = DbAdapter.SqlServer,
                    TablesToIgnore = ["__EFMigrationsHistory"]
                });
            }
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private static void EnsureInitialized()
        {
            if (_factory == null)
            {
                _factory = new CustomWebApplicationFactory();
                _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
                _configuration = _factory.Services.GetRequiredService<IConfiguration>();

                string connectionString = _configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("DefaultConnection connection string not found.");

                _checkpoint = Respawner.CreateAsync(connectionString, new RespawnerOptions
                {
                    DbAdapter = DbAdapter.SqlServer,
                    TablesToIgnore = ["__EFMigrationsHistory"]
                }).GetAwaiter().GetResult();
            }
        }

        public static async Task ResetState()
        {
            EnsureInitialized();
            string connectionString = _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection connection string not found.");

            await _checkpoint.ResetAsync(connectionString);

            _currentUserId = string.Empty;
        }

        public static string GetCurrentUserId() => _currentUserId;

        public static async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            EnsureInitialized();
            using var scope = _scopeFactory.CreateScope();

            ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.Add(entity);

            await context.SaveChangesAsync();
        }

        public static async Task<int> CountAsync<TEntity>() where TEntity : class
        {
            EnsureInitialized();
            using var scope = _scopeFactory.CreateScope();

            ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            return await context.Set<TEntity>().CountAsync();
        }

        public static async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues) where TEntity : class
        {
            EnsureInitialized();
            using var scope = _scopeFactory.CreateScope();

            ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            return await context.FindAsync<TEntity>(keyValues);
        }

        public static async Task<string> RunAsUserAsync(string userName, string password, string[] roles)
        {
            EnsureInitialized();
            using var scope = _scopeFactory.CreateScope();

            UserManager<AppUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            RoleManager<AppRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

            AppUser user = new() { UserName = userName, Email = userName };

            IdentityResult result = await userManager.CreateAsync(user, password);

            if (roles.Length != 0)
            {
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new AppRole { Name = role });
                    }
                }
                await userManager.AddToRolesAsync(user, roles);
            }

            if (result.Succeeded)
            {
                _currentUserId = user.Id;

                return _currentUserId;
            }

            throw new Exception($"Unable to create {userName}: {string.Join(Environment.NewLine, result.ToApplicationResult().Errors)}");
        }

        public static Task<string> RunAsAdministratorAsync()
            => RunAsUserAsync("admin@test.com", "Admin_123!", [Domain.Enums.Roles.Admin.ToString()]);

        public static async Task SendAsync(IBaseRequest request)
        {
            EnsureInitialized();
            using var scope = _scopeFactory.CreateScope();

            ISender sender = scope.ServiceProvider.GetRequiredService<ISender>();

            await sender.Send(request);
        }

        public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            EnsureInitialized();
            using var scope = _scopeFactory.CreateScope();

            ISender sender = scope.ServiceProvider.GetRequiredService<ISender>();

            return await sender.Send(request);
        }

        public static async Task AssignRoleToUserAsync(string userId, string roleName)
        {
            EnsureInitialized();
            using var scope = _scopeFactory.CreateScope();

            UserManager<AppUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            RoleManager<AppRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

            // Create role if it doesn't exist
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new AppRole { Name = roleName });
            }

            // Find user and assign role
            AppUser? user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await userManager.AddToRoleAsync(user, roleName);
            }
        }
    }
}