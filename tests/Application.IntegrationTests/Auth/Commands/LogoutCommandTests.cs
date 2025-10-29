using Application.Auth.Commands.Logout;
using Application.Exceptions;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;

namespace Application.IntegrationTests.Auth.Commands;

using static Testing;

[Collection("Integration Tests")]
public class LogoutCommandTests : BaseTestFixture
{
    private const string TestRefreshToken = "test_refresh_token_value_that_is_long_enough_for_validation";

    [Fact]
    public async Task ShouldLogoutSuccessfully_WhenValidRefreshTokenProvided()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password = "Password_123!";
        string userId = await RunAsUserAsync("testuser@example.com", password, [Roles.RegularUser.ToString()]);

        // Create refresh token directly
        RefreshToken refreshToken = new(userId, TestRefreshToken);
        await AddAsync(refreshToken);

        // Verify refresh token exists
        RefreshToken? refreshTokenBefore = await FindAsync<RefreshToken>(userId, TestRefreshToken);
        refreshTokenBefore.Should().NotBeNull();

        // Current user context is already set from RunAsUserAsync above

        LogoutCommand logoutCommand = new()
        {
            RefreshToken = TestRefreshToken
        };

        // Act
        await SendAsync(logoutCommand);

        // Assert
        RefreshToken? refreshTokenAfter = await FindAsync<RefreshToken>(userId, TestRefreshToken);
        refreshTokenAfter.Should().BeNull();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenRefreshTokenIsEmpty()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password = "Password_123!";
        await RunAsUserAsync("testuser@example.com", password, [Roles.RegularUser.ToString()]);

        LogoutCommand command = new()
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
        await EnsureRolesExistAsync();

        string password = "Password_123!";
        await RunAsUserAsync("testuser@example.com", password, [Roles.RegularUser.ToString()]);

        LogoutCommand command = new()
        {
            RefreshToken = null!
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowNotFoundException_WhenRefreshTokenDoesNotExist()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password = "Password_123!";
        await RunAsUserAsync("testuser@example.com", password, [Roles.RegularUser.ToString()]);

        LogoutCommand command = new()
        {
            RefreshToken = "non_existent_refresh_token_that_is_long_enough"
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*Entity \"AppUser\" by (*) was not found.*");
    }

    [Fact]
    public async Task ShouldThrowNotFoundException_WhenRefreshTokenBelongsToDifferentUser()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password1 = "Password_123!";
        string password2 = "Password_456!";

        // Create first user and add refresh token for them
        string userId1 = await RunAsUserAsync("user1@example.com", password1, [Roles.RegularUser.ToString()]);
        RefreshToken refreshTokenUser1 = new(userId1, TestRefreshToken);
        await AddAsync(refreshTokenUser1);

        // Create second user and set as current user
        string userId2 = await RunAsUserAsync("user2@example.com", password2, [Roles.RegularUser.ToString()]);

        // Try to logout with first user's refresh token while logged in as second user
        LogoutCommand command = new()
        {
            RefreshToken = TestRefreshToken
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*Entity \"AppUser\" by (*) was not found.*");
    }

    [Fact]
    public async Task ShouldRemoveRefreshTokenFromDatabase_WhenLogoutIsSuccessful()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password = "Password_123!";
        string userId = await RunAsUserAsync("testuser@example.com", password, [Roles.RegularUser.ToString()]);

        // Create refresh token directly
        RefreshToken refreshToken = new(userId, TestRefreshToken);
        await AddAsync(refreshToken);

        // Verify refresh token exists before logout
        RefreshToken? refreshTokenBefore = await FindAsync<RefreshToken>(userId, TestRefreshToken);
        refreshTokenBefore.Should().NotBeNull();
        refreshTokenBefore!.UserId.Should().Be(userId);
        refreshTokenBefore.Token.Should().Be(TestRefreshToken);

        // Current user context is already set from RunAsUserAsync above

        LogoutCommand logoutCommand = new()
        {
            RefreshToken = TestRefreshToken
        };

        // Act
        await SendAsync(logoutCommand);

        // Assert - refresh token should be removed
        RefreshToken? refreshTokenAfter = await FindAsync<RefreshToken>(userId, TestRefreshToken);
        refreshTokenAfter.Should().BeNull();
    }

    [Fact]
    public async Task ShouldLogoutSuccessfully_WithMinimumLengthRefreshToken()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password = "Password_123!";
        string userId = await RunAsUserAsync("testuser@example.com", password, [Roles.RegularUser.ToString()]);

        // Create refresh token with minimum length (10 characters)
        string minimumLengthToken = "1234567890"; // Exactly 10 characters
        RefreshToken refreshToken = new(userId, minimumLengthToken);
        await AddAsync(refreshToken);

        // Current user context is already set from RunAsUserAsync above

        LogoutCommand logoutCommand = new()
        {
            RefreshToken = minimumLengthToken
        };

        // Act
        await SendAsync(logoutCommand);

        // Assert
        RefreshToken? refreshTokenAfter = await FindAsync<RefreshToken>(userId, minimumLengthToken);
        refreshTokenAfter.Should().BeNull();
    }

    [Fact]
    public async Task ShouldLogoutSuccessfully_WhenUserHasAdminRole()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password = "Password_123!";
        string userId = await RunAsUserAsync("admin@example.com", password, [Roles.Admin.ToString()]);

        // Create refresh token directly
        RefreshToken refreshToken = new(userId, TestRefreshToken);
        await AddAsync(refreshToken);

        // Current user context is already set from RunAsUserAsync above

        LogoutCommand logoutCommand = new()
        {
            RefreshToken = TestRefreshToken
        };

        // Act
        await SendAsync(logoutCommand);

        // Assert
        RefreshToken? refreshTokenAfter = await FindAsync<RefreshToken>(userId, TestRefreshToken);
        refreshTokenAfter.Should().BeNull();
    }

    [Fact]
    public async Task ShouldLogoutSuccessfully_WhenUserHasMultipleRoles()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password = "Password_123!";
        string userId = await RunAsUserAsync("multiuser@example.com", password, [Roles.Admin.ToString(), Roles.RegularUser.ToString()]);

        // Create refresh token directly
        RefreshToken refreshToken = new(userId, TestRefreshToken);
        await AddAsync(refreshToken);

        // Current user context is already set from RunAsUserAsync above

        LogoutCommand logoutCommand = new()
        {
            RefreshToken = TestRefreshToken
        };

        // Act
        await SendAsync(logoutCommand);

        // Assert
        RefreshToken? refreshTokenAfter = await FindAsync<RefreshToken>(userId, TestRefreshToken);
        refreshTokenAfter.Should().BeNull();
    }

    [Fact]
    public async Task ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthenticated()
    {
        // Arrange
        await ResetState();

        LogoutCommand command = new()
        {
            RefreshToken = "valid_refresh_token_format_that_is_long_enough"
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}