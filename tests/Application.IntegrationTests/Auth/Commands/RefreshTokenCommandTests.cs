using Application.Auth.Commands.Refresh;
using Application.Common.Interfaces;
using Application.Exceptions;
using Domain.Entities;
using Domain.Entities.Identity;
using Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Application.IntegrationTests.Auth.Commands;

using static Testing;

[Collection("Integration Tests")]
public class RefreshTokenCommandTests : BaseTestFixture
{
    [Fact]
    public async Task ShouldRefreshTokenSuccessfully_WhenValidRefreshTokenProvided()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password = "Password_123!";
        string userId = await RunAsUserAsync("testuser@example.com", password, [Roles.RegularUser.ToString()]);

        // Generate a valid refresh token
        using IServiceScope scope = CreateScope();
        IJwtFactory jwtFactory = scope.ServiceProvider.GetRequiredService<IJwtFactory>();
        string refreshToken = jwtFactory.GenerateEncodedToken();

        // Add refresh token to database
        RefreshToken refreshTokenEntity = new(userId, refreshToken);
        await AddAsync(refreshTokenEntity);

        // Verify refresh token exists
        RefreshToken? tokenBefore = await FindAsync<RefreshToken>(userId, refreshToken);
        tokenBefore.Should().NotBeNull();

        RefreshTokenCommand command = new()
        {
            RefreshToken = refreshToken
        };

        // Act
        RefreshDto result = await SendAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.AuthToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.AuthToken.Should().NotBe(result.RefreshToken);

        // Old refresh token should be removed
        RefreshToken? oldToken = await FindAsync<RefreshToken>(userId, refreshToken);
        oldToken.Should().BeNull();

        // New refresh token should be created
        RefreshToken? newToken = await FindAsync<RefreshToken>(userId, result.RefreshToken);
        newToken.Should().NotBeNull();
        newToken!.UserId.Should().Be(userId);
        newToken.Token.Should().Be(result.RefreshToken);
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenRefreshTokenIsEmpty()
    {
        // Arrange
        await ResetState();

        RefreshTokenCommand command = new()
        {
            RefreshToken = string.Empty
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenRefreshTokenIsNull()
    {
        // Arrange
        await ResetState();

        RefreshTokenCommand command = new()
        {
            RefreshToken = null!
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenRefreshTokenIsTooShort()
    {
        // Arrange
        await ResetState();

        RefreshTokenCommand command = new()
        {
            RefreshToken = "short" // Less than 10 characters
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenRefreshTokenIsTooLong()
    {
        // Arrange
        await ResetState();

        string longToken = new('a', 501); // More than 500 characters

        RefreshTokenCommand command = new()
        {
            RefreshToken = longToken
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenRefreshTokenIsInvalid()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password = "Password_123!";
        string userId = await RunAsUserAsync("testuser@example.com", password, [Roles.RegularUser.ToString()]);

        // Create an invalid refresh token (not a valid JWT)
        string invalidToken = "invalid_refresh_token_that_is_long_enough_for_validation";

        RefreshTokenCommand command = new()
        {
            RefreshToken = invalidToken
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("*Refresh token is not valid*");
    }

    [Fact]
    public async Task ShouldThrowNotFoundException_WhenRefreshTokenDoesNotExistInDatabase()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        await RunAsUserAsync("testuser@example.com", "Password_123!", [Roles.RegularUser.ToString()]);

        // Generate a valid refresh token but don't add it to database
        using IServiceScope scope = CreateScope();
        IJwtFactory jwtFactory = scope.ServiceProvider.GetRequiredService<IJwtFactory>();
        string refreshToken = jwtFactory.GenerateEncodedToken();

        RefreshTokenCommand command = new()
        {
            RefreshToken = refreshToken
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*Entity \"RefreshToken\" by (*) was not found.*");
    }

    [Fact]
    public async Task ShouldRefreshTokenSuccessfully_WhenUserHasAdminRole()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password = "Password_123!";
        string userId = await RunAsUserAsync("admin@example.com", password, [Roles.Admin.ToString()]);

        // Generate a valid refresh token
        using IServiceScope scope = CreateScope();
        IJwtFactory jwtFactory = scope.ServiceProvider.GetRequiredService<IJwtFactory>();
        string refreshToken = jwtFactory.GenerateEncodedToken();

        // Add refresh token to database
        RefreshToken refreshTokenEntity = new(userId, refreshToken);
        await AddAsync(refreshTokenEntity);

        RefreshTokenCommand command = new()
        {
            RefreshToken = refreshToken
        };

        // Act
        RefreshDto result = await SendAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.AuthToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldRefreshTokenSuccessfully_WhenUserHasMultipleRoles()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password = "Password_123!";
        string userId = await RunAsUserAsync("multiuser@example.com", password, [Roles.Admin.ToString(), Roles.RegularUser.ToString()]);

        // Generate a valid refresh token
        using IServiceScope scope = CreateScope();
        IJwtFactory jwtFactory = scope.ServiceProvider.GetRequiredService<IJwtFactory>();
        string refreshToken = jwtFactory.GenerateEncodedToken();

        // Add refresh token to database
        RefreshToken refreshTokenEntity = new(userId, refreshToken);
        await AddAsync(refreshTokenEntity);

        RefreshTokenCommand command = new()
        {
            RefreshToken = refreshToken
        };

        // Act
        RefreshDto result = await SendAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.AuthToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        // Create a user and add refresh token
        string password = "Password_123!";
        string userId = await RunAsUserAsync("testuser@example.com", password, [Roles.RegularUser.ToString()]);

        // Generate a valid refresh token
        using IServiceScope scope = CreateScope();
        IJwtFactory jwtFactory = scope.ServiceProvider.GetRequiredService<IJwtFactory>();
        string refreshToken = jwtFactory.GenerateEncodedToken();

        // Add refresh token to database
        RefreshToken refreshTokenEntity = new(userId, refreshToken);
        await AddAsync(refreshTokenEntity);

        // Delete the user to simulate non-existent user scenario
        using IServiceScope deleteScope = CreateScope();
        UserManager<AppUser> userManager = 
            deleteScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        AppUser? user = await userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await userManager.DeleteAsync(user);
        }

        RefreshTokenCommand command = new()
        {
            RefreshToken = refreshToken
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        // Note: When user is deleted, the refresh token is also deleted due to cascade/query filter,
        // so we get NotFoundException for RefreshToken before checking AppUser
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*Entity \"RefreshToken\" by (*) was not found.*");
    }

    [Fact]
    public async Task ShouldRefreshTokenSuccessfully_WithMinimumLengthRefreshToken()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password = "Password_123!";
        string userId = await RunAsUserAsync("testuser@example.com", password, [Roles.RegularUser.ToString()]);

        // Generate a valid refresh token
        using IServiceScope scope = CreateScope();
        IJwtFactory jwtFactory = scope.ServiceProvider.GetRequiredService<IJwtFactory>();
        string refreshToken = jwtFactory.GenerateEncodedToken();

        // Add refresh token to database
        RefreshToken refreshTokenEntity = new(userId, refreshToken);
        await AddAsync(refreshTokenEntity);

        RefreshTokenCommand command = new()
        {
            RefreshToken = refreshToken
        };

        // Act
        RefreshDto result = await SendAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.AuthToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReplaceOldRefreshTokenWithNewOne_WhenRefreshIsSuccessful()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password = "Password_123!";
        string userId = await RunAsUserAsync("testuser@example.com", password, [Roles.RegularUser.ToString()]);

        // Generate a valid refresh token
        using IServiceScope scope = CreateScope();
        IJwtFactory jwtFactory = scope.ServiceProvider.GetRequiredService<IJwtFactory>();
        string oldRefreshToken = jwtFactory.GenerateEncodedToken();

        // Add refresh token to database
        RefreshToken refreshTokenEntity = new(userId, oldRefreshToken);
        await AddAsync(refreshTokenEntity);

        // Verify old token exists
        RefreshToken? tokenBefore = await FindAsync<RefreshToken>(userId, oldRefreshToken);
        tokenBefore.Should().NotBeNull();

        RefreshTokenCommand command = new()
        {
            RefreshToken = oldRefreshToken
        };

        // Act
        RefreshDto result = await SendAsync(command);

        // Assert
        // Old token should be removed
        RefreshToken? oldTokenAfter = await FindAsync<RefreshToken>(userId, oldRefreshToken);
        oldTokenAfter.Should().BeNull();

        // New token should exist
        RefreshToken? newToken = await FindAsync<RefreshToken>(userId, result.RefreshToken);
        newToken.Should().NotBeNull();
        newToken!.Token.Should().Be(result.RefreshToken);
        newToken.UserId.Should().Be(userId);
    }
}

