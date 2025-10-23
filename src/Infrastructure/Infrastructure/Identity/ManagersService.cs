using Application.Common.Interfaces;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Infrastructure.Identity
{
    public class ManagersService(
        IAuthorizationService authorizationService,
        IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory,
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager) : IManagersService
    {
        public async Task<AppUser?> AuthenticateAsync(string userName, string password, CancellationToken cancellationToken = default)
        {
            AppUser? user = await FindByUserNameOrEmailAsync(userName, cancellationToken);

            if (user == null)
                return null;

            SignInResult result = await signInManager.PasswordSignInAsync(user.UserName ?? string.Empty, password, false, false);

            return result.Succeeded ? user : null;
        }

        public async Task<bool> AuthorizeAsync(string userId, string policyName, CancellationToken cancellationToken = default)
        {
            AppUser? user = await userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

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
                                .Where(x => x.Email == term || x.UserName == term)
                                .Where(x => !x.IsDeleted)
                                .FirstOrDefaultAsync(cancellationToken);

        public async Task<string[]> GetRolesAsync(AppUser user)
            => [.. await userManager.GetRolesAsync(user)];

        public async Task SignOutAsync()
            => await signInManager.SignOutAsync();

        public async Task<AppUser?> FindByIdAsync(string id)
            => await userManager.FindByIdAsync(id);
    }
}