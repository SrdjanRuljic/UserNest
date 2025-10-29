using Application.Exceptions;
using Application.Users.Queries.ValidatePassword;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Entities.Identity;
using Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.IntegrationTests.Users.Queries;

using static Testing;

[Collection("Integration Tests")]
public class ValidatePasswordQueryTests : BaseTestFixture
{
    [Fact]
    public async Task ShouldReturnTrue_WhenPasswordIsValid()
    {
        // Arrange
        await ResetState();
        string currentUserId = await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        // Get the current user
        AppUser? currentUser = await FindAsync<AppUser>(currentUserId);
        currentUser.Should().NotBeNull();

        // Set a known password for the user
        string testPassword = "ValidPassword123!";
        await SetUserPasswordAsync(currentUser!, testPassword);

        ValidatePasswordQuery query = new()
        {
            Password = testPassword
        };

        // Act
        bool result = await SendAsync(query);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldReturnFalse_WhenPasswordIsInvalid()
    {
        // Arrange
        await ResetState();
        string currentUserId = await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        // Get the current user
        AppUser? currentUser = await FindAsync<AppUser>(currentUserId);
        currentUser.Should().NotBeNull();

        // Set a known password for the user
        string correctPassword = "ValidPassword123!";
        await SetUserPasswordAsync(currentUser!, correctPassword);

        ValidatePasswordQuery query = new()
        {
            Password = "WrongPassword123!"
        };

        // Act
        bool result = await SendAsync(query);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenPasswordIsEmpty()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        ValidatePasswordQuery query = new()
        {
            Password = string.Empty
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(query);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenPasswordIsNull()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        ValidatePasswordQuery query = new()
        {
            Password = null!
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(query);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenPasswordIsTooShort()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        ValidatePasswordQuery query = new()
        {
            Password = "12345" // Less than 6 characters
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(query);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenPasswordDoesNotMeetComplexityRequirements()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        ValidatePasswordQuery query = new()
        {
            Password = "simplepassword" // No uppercase, numbers, or special characters
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(query);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenPasswordIsTooLong()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        ValidatePasswordQuery query = new()
        {
            Password = new string('A', 101) // More than 100 characters
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(query);
        await act.Should().ThrowAsync<BadRequestException>();
    }


    [Fact]
    public async Task ShouldReturnTrue_WhenPasswordMeetsAllComplexityRequirements()
    {
        // Arrange
        await ResetState();
        string currentUserId = await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        // Get the current user
        AppUser? currentUser = await FindAsync<AppUser>(currentUserId);
        currentUser.Should().NotBeNull();

        // Set a password that meets all complexity requirements
        string complexPassword = "ComplexP@ssw0rd!";
        await SetUserPasswordAsync(currentUser!, complexPassword);

        ValidatePasswordQuery query = new()
        {
            Password = complexPassword
        };

        // Act
        bool result = await SendAsync(query);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenPasswordIsWhitespaceOnly()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        ValidatePasswordQuery query = new()
        {
            Password = "   " // Whitespace only
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(query);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    private static async Task SetUserPasswordAsync(AppUser user, string password)
    {
        using var scope = CreateScope();
        UserManager<AppUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        
        // Get a fresh instance of the user to avoid tracking conflicts
        AppUser? freshUser = await userManager.FindByIdAsync(user.Id);
        if (freshUser == null)
        {
            throw new InvalidOperationException($"User with ID {user.Id} not found");
        }
        
        string token = await userManager.GeneratePasswordResetTokenAsync(freshUser);
        IdentityResult result = await userManager.ResetPasswordAsync(freshUser, token, password);
        
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to set password: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}
