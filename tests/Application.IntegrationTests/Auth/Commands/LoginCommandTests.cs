using Application.Auth.Commands.Login;
using Application.Exceptions;
using Domain.Entities;
using Domain.Entities.Identity;
using Domain.Enums;
using FluentAssertions;

namespace Application.IntegrationTests.Auth.Commands;

using static Testing;

[Collection("Integration Tests")]
public class LoginCommandTests : BaseTestFixture
{
    [Fact]
    public async Task ShouldLoginSuccessfully_WhenValidCredentialsProvided()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password = "Password_123!";
        string userId = await RunAsUserAsync("testuser@example.com", password, [Roles.RegularUser.ToString()]);

        LoginCommand command = new()
        {
            Username = "testuser@example.com",
            Password = password
        };

        // Act
        LoginDto result = await SendAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.AuthToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();

        RefreshToken? refreshToken = await FindAsync<RefreshToken>(userId, result.RefreshToken);
        refreshToken.Should().NotBeNull();
        refreshToken!.UserId.Should().Be(userId);
        refreshToken.Token.Should().Be(result.RefreshToken);
    }

    [Fact]
    public async Task ShouldLoginSuccessfully_WhenUsingUsernameInsteadOfEmail()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password = "Password_123!";
        string username = "testusername";
        string userId = await RunAsUserAsync($"{username}@example.com", password, [Roles.RegularUser.ToString()]);

        await UpdateUserAsync(userId, user => user.UserName = username);

        LoginCommand command = new()
        {
            Username = username,
            Password = password
        };

        // Act
        LoginDto result = await SendAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.AuthToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenUsernameIsEmpty()
    {
        // Arrange
        await ResetState();

        LoginCommand command = new()
        {
            Username = string.Empty,
            Password = "Password_123!"
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenPasswordIsEmpty()
    {
        // Arrange
        await ResetState();

        LoginCommand command = new()
        {
            Username = "testuser@example.com",
            Password = string.Empty
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenUsernameIsTooShort()
    {
        // Arrange
        await ResetState();

        LoginCommand command = new()
        {
            Username = "ab", // Less than 3 characters
            Password = "Password_123!"
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenUsernameIsTooLong()
    {
        // Arrange
        await ResetState();

        string longUsername = new string('a', 257); // More than 256 characters

        LoginCommand command = new()
        {
            Username = longUsername,
            Password = "Password_123!"
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenPasswordIsTooShort()
    {
        // Arrange
        await ResetState();

        LoginCommand command = new()
        {
            Username = "testuser@example.com",
            Password = "Pass1" // Less than 6 characters
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenPasswordIsTooLong()
    {
        // Arrange
        await ResetState();

        string longPassword = new string('a', 101); // More than 100 characters

        LoginCommand command = new()
        {
            Username = "testuser@example.com",
            Password = longPassword
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenInvalidUsername()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        LoginCommand command = new()
        {
            Username = "nonexistent@example.com",
            Password = "Password_123!"
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenInvalidPassword()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password = "Password_123!";
        await RunAsUserAsync("testuser@example.com", password, [Roles.RegularUser.ToString()]);

        LoginCommand command = new()
        {
            Username = "testuser@example.com",
            Password = "WrongPassword_123!"
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldSaveRefreshToken_WhenLoginIsSuccessful()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password = "Password_123!";
        string userId = await RunAsUserAsync("testuser@example.com", password, [Roles.RegularUser.ToString()]);

        LoginCommand command = new()
        {
            Username = "testuser@example.com",
            Password = password
        };

        // Act
        LoginDto result = await SendAsync(command);

        // Assert
        RefreshToken? refreshToken = await FindAsync<RefreshToken>(userId, result.RefreshToken);
        refreshToken.Should().NotBeNull();
        refreshToken!.UserId.Should().Be(userId);
        refreshToken.Token.Should().Be(result.RefreshToken);
    }

    [Fact]
    public async Task ShouldReturnValidTokens_WhenLoginIsSuccessful()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password = "Password_123!";
        await RunAsUserAsync("testuser@example.com", password, [Roles.RegularUser.ToString()]);

        LoginCommand command = new()
        {
            Username = "testuser@example.com",
            Password = password
        };

        // Act
        LoginDto result = await SendAsync(command);

        // Assert
        result.AuthToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        
        // Tokens should be different
        result.AuthToken.Should().NotBe(result.RefreshToken);
    }

    [Fact]
    public async Task ShouldLoginSuccessfully_WhenUserHasAdminRole()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password = "Password_123!";
        string userId = await RunAsUserAsync("adminuser@example.com", password, [Roles.Admin.ToString()]);

        LoginCommand command = new()
        {
            Username = "adminuser@example.com",
            Password = password
        };

        // Act
        LoginDto result = await SendAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.AuthToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldLoginSuccessfully_WhenUserHasMultipleRoles()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password = "Password_123!";
        string userId = await RunAsUserAsync("multiuser@example.com", password, [Roles.Admin.ToString(), Roles.RegularUser.ToString()]);

        LoginCommand command = new()
        {
            Username = "multiuser@example.com",
            Password = password
        };

        // Act
        LoginDto result = await SendAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.AuthToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldLoginSuccessfully_WithMinimumLengthUsername()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password = "Password_123!";
        string username = "abc";
        string userId = await RunAsUserAsync($"{username}@example.com", password, [Roles.RegularUser.ToString()]);

        await UpdateUserAsync(userId, user => user.UserName = username);

        LoginCommand command = new()
        {
            Username = username,
            Password = password
        };

        // Act
        LoginDto result = await SendAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.AuthToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldLoginSuccessfully_WithMinimumLengthPassword()
    {
        // Arrange
        await ResetState();
        await EnsureRolesExistAsync();

        string password = "Pass1!";
        string userId = await RunAsUserAsync("testuser@example.com", password, [Roles.RegularUser.ToString()]);

        LoginCommand command = new()
        {
            Username = "testuser@example.com",
            Password = password
        };

        // Act
        LoginDto result = await SendAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.AuthToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenUsernameIsNull()
    {
        // Arrange
        await ResetState();

        LoginCommand command = new()
        {
            Username = null!,
            Password = "Password_123!"
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenPasswordIsNull()
    {
        // Arrange
        await ResetState();

        LoginCommand command = new()
        {
            Username = "testuser@example.com",
            Password = null!
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }
}

