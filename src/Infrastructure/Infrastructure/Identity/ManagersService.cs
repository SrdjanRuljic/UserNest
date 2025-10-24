using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities.Identity;
using Infrastructure.Identity.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

namespace Infrastructure.Identity
{
    public class ManagersService(
        IAuthorizationService authorizationService,
        IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory,
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager) : IManagersService
    {
        public async Task<AppUser?> AuthenticateAsync(
            string userName,
            string password,
            CancellationToken cancellationToken = default)
        {
            AppUser? user = await FindByUserNameOrEmailAsync(userName, cancellationToken);

            if (user == null)
                return null;

            SignInResult result = await signInManager.PasswordSignInAsync(
                user.UserName ?? string.Empty,
                password,
                false,
                false);

            return result.Succeeded ? user : null;
        }

        public async Task<bool> AuthorizeAsync(
            string userId,
            string policyName,
            CancellationToken cancellationToken = default)
        {
            AppUser? user = await userManager.Users
                .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
                return false;

            ClaimsPrincipal principal = await userClaimsPrincipalFactory.CreateAsync(user);

            AuthorizationResult result = await authorizationService.AuthorizeAsync(principal, policyName);

            return result.Succeeded;
        }

        public async Task<bool> IsInRoleAsync(string userId, string role)
        {
            AppUser? user = await userManager.FindByIdAsync(userId);

            return user != null && await userManager.IsInRoleAsync(user, role);
        }

        public async Task<AppUser?> FindByUserNameOrEmailAsync(string term, CancellationToken cancellationToken)
            => await userManager.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .Include(x => x.Language)
                .Where(x => x.Email == term || x.UserName == term)
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<string[]> GetRolesAsync(AppUser user)
            => [.. await userManager.GetRolesAsync(user)];

        public async Task SignOutAsync()
            => await signInManager.SignOutAsync();

        public async Task<AppUser?> FindByIdAsync(string id)
            => await userManager.FindByIdAsync(id);

        public async Task<(Result Result, string Id)> CreateUser(
            AppUser user,
            string password,
            string role)
        {
            IdentityResult createResult = await userManager.CreateAsync(user, password);

            if (!createResult.Succeeded)
                return (createResult.ToApplicationResult(), user.Id);

            IdentityResult roleResult = await userManager.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
            {
                await userManager.DeleteAsync(user);

                return (roleResult.ToApplicationResult(), user.Id);
            }

            return (IdentityResult.Success.ToApplicationResult(), user.Id);
        }

        public async Task<bool> UserExistsAsync(
            string username,
            string email,
            CancellationToken cancellationToken = default)
            => await userManager.Users
                .Where(x => !x.IsDeleted)
                .AnyAsync(x => x.UserName == username || x.Email == email, cancellationToken);

        public async Task<Result> UpdateUser(AppUser user)
        {
            IdentityResult result = await userManager.UpdateAsync(user);

            return result.ToApplicationResult();
        }

        public IQueryable<AppUser> GetUsers() =>
            userManager.Users;
    }
}