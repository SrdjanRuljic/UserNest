using Application.Exceptions;
using Application.Users.Commands.Insert;
using Domain.Entities;
using Domain.Entities.Identity;
using Domain.Enums;
using FluentAssertions;

namespace Application.IntegrationTests.Users.Commands;

using static Testing;

[Collection("Integration Tests")]
public class InsertUserCommandTests : BaseTestFixture
{
    [Fact]
    public async Task ShouldInsertUser_WhenValidDataProvided()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        // Create a language first
        Language language = new()
        {
            Name = "English",
            Culture = "en-US",
            Description = "English language",
            IsDeleted = false
        };
        await AddAsync(language);

        InsertUserCommand command = new()
        {
            UserName = "newuser",
            Email = "newuser@example.com",
            Password = "Password_123!",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "+1234567890",
            LanguageId = language.Id,
            Role = Roles.RegularUser.ToString()
        };

        // Act
        string userId = await SendAsync(command);

        // Assert
        userId.Should().NotBeNullOrEmpty();

        AppUser? createdUser = await FindAsync<AppUser>(userId);
        createdUser.Should().NotBeNull();
        createdUser!.UserName.Should().Be(command.UserName);
        createdUser.Email.Should().Be(command.Email);
        createdUser.FirstName.Should().Be(command.FirstName);
        createdUser.LastName.Should().Be(command.LastName);
        createdUser.PhoneNumber.Should().Be(command.PhoneNumber);
        createdUser.LanguageId.Should().Be(command.LanguageId);
        createdUser.EmailConfirmed.Should().BeTrue();
        createdUser.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task ShouldInsertUser_WhenMinimalValidDataProvided()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        // Create a language first
        Language language = new()
        {
            Name = "English",
            Culture = "en-US",
            Description = "English language",
            IsDeleted = false
        };
        await AddAsync(language);

        InsertUserCommand command = new()
        {
            UserName = "minimaluser",
            Email = "minimal@example.com",
            Password = "Password_123!",
            FirstName = "Minimal",
            LastName = "User",
            Role = Roles.RegularUser.ToString()
        };

        // Act
        string userId = await SendAsync(command);

        // Assert
        userId.Should().NotBeNullOrEmpty();

        AppUser? createdUser = await FindAsync<AppUser>(userId);
        createdUser.Should().NotBeNull();
        createdUser!.UserName.Should().Be(command.UserName);
        createdUser.Email.Should().Be(command.Email);
        createdUser.FirstName.Should().Be(command.FirstName);
        createdUser.LastName.Should().Be(command.LastName);
        createdUser.PhoneNumber.Should().BeNull();
        createdUser.LanguageId.Should().BeNull();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenUserNameIsEmpty()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        InsertUserCommand command = new()
        {
            UserName = string.Empty,
            Email = "test@example.com",
            Password = "Password_123!",
            FirstName = "Test",
            LastName = "User",
            Role = Roles.RegularUser.ToString()
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenEmailIsEmpty()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        InsertUserCommand command = new()
        {
            UserName = "testuser",
            Email = string.Empty,
            Password = "Password_123!",
            FirstName = "Test",
            LastName = "User",
            Role = Roles.RegularUser.ToString()
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
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        InsertUserCommand command = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = string.Empty,
            FirstName = "Test",
            LastName = "User",
            Role = Roles.RegularUser.ToString()
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenFirstNameIsEmpty()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        InsertUserCommand command = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "Password_123!",
            FirstName = string.Empty,
            LastName = "User",
            Role = Roles.RegularUser.ToString()
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenLastNameIsEmpty()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        InsertUserCommand command = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "Password_123!",
            FirstName = "Test",
            LastName = string.Empty,
            Role = Roles.RegularUser.ToString()
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenRoleIsEmpty()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        InsertUserCommand command = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "Password_123!",
            FirstName = "Test",
            LastName = "User",
            Role = string.Empty
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }


    [Fact]
    public async Task ShouldThrowBadRequestException_WhenPasswordDoesNotMeetComplexityRequirements()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        InsertUserCommand command = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "password123", // No uppercase, no special character
            FirstName = "Test",
            LastName = "User",
            Role = Roles.RegularUser.ToString()
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenEmailIsInvalid()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        InsertUserCommand command = new()
        {
            UserName = "testuser",
            Email = "invalid-email",
            Password = "Password_123!",
            FirstName = "Test",
            LastName = "User",
            Role = Roles.RegularUser.ToString()
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenPhoneNumberIsInvalid()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        InsertUserCommand command = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "Password_123!",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "invalid-phone",
            Role = Roles.RegularUser.ToString()
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenRoleIsInvalid()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        InsertUserCommand command = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "Password_123!",
            FirstName = "Test",
            LastName = "User",
            Role = "InvalidRole"
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenUserNameAlreadyExists()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        // Create a language first
        Language language = new()
        {
            Name = "English",
            Culture = "en-US",
            Description = "English language",
            IsDeleted = false
        };
        await AddAsync(language);

        // Create an existing user
        AppUser existingUser = new()
        {
            UserName = "existinguser",
            Email = "existing@example.com",
            FirstName = "Existing",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };
        await AddAsync(existingUser);

        InsertUserCommand command = new()
        {
            UserName = "existinguser", // Same username
            Email = "new@example.com",
            Password = "Password_123!",
            FirstName = "New",
            LastName = "User",
            Role = Roles.RegularUser.ToString()
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenEmailAlreadyExists()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        // Create a language first
        Language language = new()
        {
            Name = "English",
            Culture = "en-US",
            Description = "English language",
            IsDeleted = false
        };
        await AddAsync(language);

        // Create an existing user
        AppUser existingUser = new()
        {
            UserName = "existinguser",
            Email = "existing@example.com",
            FirstName = "Existing",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };
        await AddAsync(existingUser);

        InsertUserCommand command = new()
        {
            UserName = "newuser",
            Email = "existing@example.com", // Same email
            Password = "Password_123!",
            FirstName = "New",
            LastName = "User",
            Role = Roles.RegularUser.ToString()
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowForbiddenAccessException_WhenUserIsNotAdmin()
    {
        // Arrange
        await ResetState();
        await RunAsUserAsync("regularuser@example.com", "Password_123!", [Roles.RegularUser.ToString()]);

        InsertUserCommand command = new()
        {
            UserName = "newuser",
            Email = "new@example.com",
            Password = "Password_123!",
            FirstName = "New",
            LastName = "User",
            Role = Roles.RegularUser.ToString()
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Fact]
    public async Task ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthenticated()
    {
        // Arrange
        await ResetState();

        InsertUserCommand command = new()
        {
            UserName = "newuser",
            Email = "new@example.com",
            Password = "Password_123!",
            FirstName = "New",
            LastName = "User",
            Role = Roles.RegularUser.ToString()
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task ShouldInsertUserWithAdminRole_WhenAdminRoleSpecified()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        // Create a language first
        Language language = new()
        {
            Name = "English",
            Culture = "en-US",
            Description = "English language",
            IsDeleted = false
        };
        await AddAsync(language);

        InsertUserCommand command = new()
        {
            UserName = "adminuser",
            Email = "admin@example.com",
            Password = "Password_123!",
            FirstName = "Admin",
            LastName = "User",
            LanguageId = language.Id,
            Role = Roles.Admin.ToString()
        };

        // Act
        string userId = await SendAsync(command);

        // Assert
        userId.Should().NotBeNullOrEmpty();

        AppUser? createdUser = await FindAsync<AppUser>(userId);
        createdUser.Should().NotBeNull();
        createdUser!.UserName.Should().Be(command.UserName);
        createdUser.Email.Should().Be(command.Email);
    }

    [Fact]
    public async Task ShouldInsertUserWithRegularUserRole_WhenRegularUserRoleSpecified()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        // Create a language first
        Language language = new()
        {
            Name = "English",
            Culture = "en-US",
            Description = "English language",
            IsDeleted = false
        };
        await AddAsync(language);

        InsertUserCommand command = new()
        {
            UserName = "regularuser",
            Email = "regular@example.com",
            Password = "Password_123!",
            FirstName = "Regular",
            LastName = "User",
            LanguageId = language.Id,
            Role = Roles.RegularUser.ToString()
        };

        // Act
        string userId = await SendAsync(command);

        // Assert
        userId.Should().NotBeNullOrEmpty();

        AppUser? createdUser = await FindAsync<AppUser>(userId);
        createdUser.Should().NotBeNull();
        createdUser!.UserName.Should().Be(command.UserName);
        createdUser.Email.Should().Be(command.Email);
    }

    [Fact]
    public async Task ShouldInsertUserWithValidPhoneNumber_WhenPhoneNumberProvided()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        // Create a language first
        Language language = new()
        {
            Name = "English",
            Culture = "en-US",
            Description = "English language",
            IsDeleted = false
        };
        await AddAsync(language);

        InsertUserCommand command = new()
        {
            UserName = "phoneuser",
            Email = "phone@example.com",
            Password = "Password_123!",
            FirstName = "Phone",
            LastName = "User",
            PhoneNumber = "+1234567890",
            LanguageId = language.Id,
            Role = Roles.RegularUser.ToString()
        };

        // Act
        string userId = await SendAsync(command);

        // Assert
        userId.Should().NotBeNullOrEmpty();

        AppUser? createdUser = await FindAsync<AppUser>(userId);
        createdUser.Should().NotBeNull();
        createdUser!.PhoneNumber.Should().Be(command.PhoneNumber);
    }

    [Fact]
    public async Task ShouldInsertUserWithValidLanguageId_WhenLanguageIdProvided()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        // Create a language first
        Language language = new()
        {
            Name = "English",
            Culture = "en-US",
            Description = "English language",
            IsDeleted = false
        };
        await AddAsync(language);

        InsertUserCommand command = new()
        {
            UserName = "languser",
            Email = "lang@example.com",
            Password = "Password_123!",
            FirstName = "Lang",
            LastName = "User",
            LanguageId = language.Id,
            Role = Roles.RegularUser.ToString()
        };

        // Act
        string userId = await SendAsync(command);

        // Assert
        userId.Should().NotBeNullOrEmpty();

        AppUser? createdUser = await FindAsync<AppUser>(userId);
        createdUser.Should().NotBeNull();
        createdUser!.LanguageId.Should().Be(command.LanguageId);
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenLanguageIdIsInvalid()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        InsertUserCommand command = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "Password_123!",
            FirstName = "Test",
            LastName = "User",
            LanguageId = -1, // Invalid language ID
            Role = Roles.RegularUser.ToString()
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldSetCreatedFields_WhenUserIsInserted()
    {
        // Arrange
        await ResetState();
        string adminUserId = await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        // Create a language first
        Language language = new()
        {
            Name = "English",
            Culture = "en-US",
            Description = "English language",
            IsDeleted = false
        };
        await AddAsync(language);

        InsertUserCommand command = new()
        {
            UserName = "audituser",
            Email = "audit@example.com",
            Password = "Password_123!",
            FirstName = "Audit",
            LastName = "User",
            LanguageId = language.Id,
            Role = Roles.RegularUser.ToString()
        };

        // Act
        string userId = await SendAsync(command);

        // Assert
        AppUser? createdUser = await FindAsync<AppUser>(userId);
        createdUser.Should().NotBeNull();
        createdUser!.CreatedBy.Should().Be(adminUserId);
        createdUser.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task ShouldHandleLongValidInputs_WhenMaximumLengthValuesProvided()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();
        await EnsureRolesExistAsync();

        // Create a language first
        Language language = new()
        {
            Name = "English",
            Culture = "en-US",
            Description = "English language",
            IsDeleted = false
        };
        await AddAsync(language);

        string longUserName = new string('a', 256); // Maximum length
        string longEmail = new string('a', 247) + "@test.com"; // 247 + 9 = 256 characters (maximum)
        string longFirstName = new string('a', 100); // Maximum length
        string longLastName = new string('a', 100); // Maximum length
        string longPhoneNumber = "+1234567890123456"; // 16 digits (maximum allowed)

        InsertUserCommand command = new()
        {
            UserName = longUserName,
            Email = longEmail,
            Password = "Password_123!",
            FirstName = longFirstName,
            LastName = longLastName,
            PhoneNumber = longPhoneNumber,
            LanguageId = language.Id,
            Role = Roles.RegularUser.ToString()
        };

        // Act
        string userId = await SendAsync(command);

        // Assert
        userId.Should().NotBeNullOrEmpty();

        AppUser? createdUser = await FindAsync<AppUser>(userId);
        createdUser.Should().NotBeNull();
        createdUser!.UserName.Should().Be(longUserName);
        createdUser.Email.Should().Be(longEmail);
        createdUser.FirstName.Should().Be(longFirstName);
        createdUser.LastName.Should().Be(longLastName);
        createdUser.PhoneNumber.Should().Be(longPhoneNumber);
    }
}