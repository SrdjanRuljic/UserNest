using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities.Identity;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Application.IntegrationTests.Wrappers;

public class ManagersServiceWrapper(
    IAuthorizationService authorizationService,
    IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory,
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager) : IManagersService
{
    private readonly ManagersService _managersService = new(
            authorizationService,
            userClaimsPrincipalFactory,
            userManager,
            signInManager);

    public async Task<AppUser?> AuthenticateAsync(
        string userName,
        string password,
        CancellationToken cancellationToken = default)
    {
        AppUser? user = await _managersService.FindByUserNameOrEmailAsync(userName, cancellationToken);

        if (user == null)
            return null;

        bool isValidPassword = await userManager.CheckPasswordAsync(user, password);

        return isValidPassword ? user : null;
    }

    public Task<bool> AuthorizeAsync(string userId, string policyName, CancellationToken cancellationToken = default)
        => _managersService.AuthorizeAsync(userId, policyName, cancellationToken);

    public Task<AppUser?> FindByUserNameOrEmailAsync(string term, CancellationToken cancellationToken = default)
        => _managersService.FindByUserNameOrEmailAsync(term, cancellationToken);

    public Task<string[]> GetRolesAsync(AppUser user)
        => _managersService.GetRolesAsync(user);

    public Task SignOutAsync()
        => _managersService.SignOutAsync();

    public Task<AppUser?> FindByIdAsync(string id)
        => _managersService.FindByIdAsync(id);

    public Task<(Result Result, string Id)> CreateUserAsync(AppUser user, string password, string role)
        => _managersService.CreateUserAsync(user, password, role);

    public Task<bool> UserExistsAsync(string username, string email, CancellationToken cancellationToken = default)
        => _managersService.UserExistsAsync(username, email, cancellationToken);

    public Task<Result> UpdateUserAsync(AppUser user)
        => _managersService.UpdateUserAsync(user);

    public Task<Result> UpdatePasswordAsync(AppUser user, string newPassword)
        => _managersService.UpdatePasswordAsync(user, newPassword);

    public Task<bool> UserExistsExcludingAsync(string username, string email, string excludeUserId, CancellationToken cancellationToken = default)
        => _managersService.UserExistsExcludingAsync(username, email, excludeUserId, cancellationToken);

    public Task<bool> ValidatePasswordAsync(AppUser user, string password)
        => _managersService.ValidatePasswordAsync(user, password);

    public IQueryable<AppUser> GetUsers()
        => _managersService.GetUsers();

    public Task<bool> IsInRoleAsync(string userId, string role)
        => _managersService.IsInRoleAsync(userId, role);
}