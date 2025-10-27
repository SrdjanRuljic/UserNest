using Application.Common.Models;
using Domain.Entities.Identity;

namespace Application.Common.Interfaces
{
    public interface IManagersService
    {
        Task<bool> IsInRoleAsync(string userId, string role);

        Task<bool> AuthorizeAsync(string userId, string policyName, CancellationToken cancellationToken = default);

        Task<AppUser?> AuthenticateAsync(string userName, string password, CancellationToken cancellationToken = default);

        Task<AppUser?> FindByUserNameOrEmailAsync(string term, CancellationToken cancellationToken = default);

        Task<string[]> GetRolesAsync(AppUser user);

        Task SignOutAsync();

        Task<AppUser?> FindByIdAsync(string id);

        Task<(Result Result, string Id)> CreateUserAsync(AppUser user, string password, string role);

        Task<bool> UserExistsAsync(string username, string email, CancellationToken cancellationToken = default);

        Task<Result> UpdateUserAsync(AppUser user);

        Task<Result> UpdatePasswordAsync(AppUser user, string newPassword);

        Task<bool> UserExistsExcludingAsync(string username, string email, string excludeUserId, CancellationToken cancellationToken = default);

        Task<bool> ValidatePasswordAsync(AppUser user, string password);

        IQueryable<AppUser> GetUsers();
    }
}