using Application.Exceptions;
using Application.Users.Commands.Update;
using Domain.Entities;
using Domain.Entities.Identity;
using Domain.Enums;
using FluentAssertions;

namespace Application.IntegrationTests.Users.Commands;

using static Testing;

[Collection("Integration Tests")]
public class UpdateUserCommandTests : BaseTestFixture
{
    [Fact]
    public async Task ShouldUpdateUser_WhenValidDataProvided()
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

        // Create a user to update
        AppUser userToUpdate = new()
        {
            UserName = "originaluser",
            Email = "original@example.com",
            FirstName = "Original",
            LastName = "User",
            PhoneNumber = "+1234567890",
            LanguageId = language.Id,
            IsDeleted = false
        };
        await AddAsync(userToUpdate);
        await AssignRoleToUserAsync(userToUpdate.Id, Roles.RegularUser.ToString());

        UpdateUserCommand command = new()
        {
            Id = userToUpdate.Id,
            UserName = "updateduser",
            Email = "updated@example.com",
            FirstName = "Updated",
            LastName = "User",
            LanguageId = language.Id
        };

        // Act
        string userId = await SendAsync(command);

        // Assert
        userId.Should().Be(userToUpdate.Id);

        AppUser? updatedUser = await FindAsync<AppUser>(userToUpdate.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.UserName.Should().Be(command.UserName);
        updatedUser.Email.Should().Be(command.Email);
        updatedUser.FirstName.Should().Be(command.FirstName);
        updatedUser.LastName.Should().Be(command.LastName);
        updatedUser.LanguageId.Should().Be(command.LanguageId);
        updatedUser.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task ShouldUpdateUser_WhenOnlyFirstNameProvided()
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

        // Create a user to update
        AppUser userToUpdate = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Original",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };
        await AddAsync(userToUpdate);
        await AssignRoleToUserAsync(userToUpdate.Id, Roles.RegularUser.ToString());

        UpdateUserCommand command = new()
        {
            Id = userToUpdate.Id,
            FirstName = "UpdatedFirstName"
        };

        // Act
        string userId = await SendAsync(command);

        // Assert
        userId.Should().Be(userToUpdate.Id);

        AppUser? updatedUser = await FindAsync<AppUser>(userToUpdate.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.FirstName.Should().Be(command.FirstName);
        updatedUser.LastName.Should().Be("User"); // Should remain unchanged
        updatedUser.UserName.Should().Be("testuser"); // Should remain unchanged
        updatedUser.Email.Should().Be("test@example.com"); // Should remain unchanged
    }

    [Fact]
    public async Task ShouldUpdateUserPassword_WhenPasswordProvided()
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

        // Create a user to update
        AppUser userToUpdate = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };
        await AddAsync(userToUpdate);
        await AssignRoleToUserAsync(userToUpdate.Id, Roles.RegularUser.ToString());

        UpdateUserCommand command = new()
        {
            Id = userToUpdate.Id,
            Password = "NewPassword_123!"
        };

        // Act
        string userId = await SendAsync(command);

        // Assert
        userId.Should().Be(userToUpdate.Id);

        AppUser? updatedUser = await FindAsync<AppUser>(userToUpdate.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.FirstName.Should().Be("Test"); // Should remain unchanged
        updatedUser.LastName.Should().Be("User"); // Should remain unchanged
    }

    [Fact]
    public async Task ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();

        string nonExistentUserId = Guid.NewGuid().ToString();
        UpdateUserCommand command = new()
        {
            Id = nonExistentUserId,
            FirstName = "Updated"
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"AppUser\" by ({nonExistentUserId}) was not found.");
    }

    [Fact]
    public async Task ShouldThrowNotFoundException_WhenUserIsDeleted()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();

        // Create a language first
        Language language = new()
        {
            Name = "English",
            Culture = "en-US",
            Description = "English language",
            IsDeleted = false
        };
        await AddAsync(language);

        // Create a deleted user
        AppUser deletedUser = new()
        {
            UserName = "deleteduser",
            Email = "deleted@example.com",
            FirstName = "Deleted",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = true
        };
        await AddAsync(deletedUser);

        UpdateUserCommand command = new()
        {
            Id = deletedUser.Id,
            FirstName = "Updated"
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"AppUser\" by ({deletedUser.Id}) was not found.");
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenUserIdIsEmpty()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();

        UpdateUserCommand command = new()
        {
            Id = string.Empty,
            FirstName = "Updated"
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenUserIdIsNull()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();

        UpdateUserCommand command = new()
        {
            Id = null!,
            FirstName = "Updated"
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenFirstNameExceedsMaxLength()
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

        // Create a user to update
        AppUser userToUpdate = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };
        await AddAsync(userToUpdate);
        await AssignRoleToUserAsync(userToUpdate.Id, Roles.RegularUser.ToString());

        UpdateUserCommand command = new()
        {
            Id = userToUpdate.Id,
            FirstName = new string('a', 101) // Exceeds max length of 100
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenLastNameExceedsMaxLength()
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

        // Create a user to update
        AppUser userToUpdate = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };
        await AddAsync(userToUpdate);
        await AssignRoleToUserAsync(userToUpdate.Id, Roles.RegularUser.ToString());

        UpdateUserCommand command = new()
        {
            Id = userToUpdate.Id,
            LastName = new string('a', 101) // Exceeds max length of 100
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenUserNameExceedsMaxLength()
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

        // Create a user to update
        AppUser userToUpdate = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };
        await AddAsync(userToUpdate);
        await AssignRoleToUserAsync(userToUpdate.Id, Roles.RegularUser.ToString());

        UpdateUserCommand command = new()
        {
            Id = userToUpdate.Id,
            UserName = new string('a', 257) // Exceeds max length of 256
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenUserNameIsTooShort()
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

        // Create a user to update
        AppUser userToUpdate = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };
        await AddAsync(userToUpdate);
        await AssignRoleToUserAsync(userToUpdate.Id, Roles.RegularUser.ToString());

        UpdateUserCommand command = new()
        {
            Id = userToUpdate.Id,
            UserName = "ab" // Too short (minimum is 3)
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

        // Create a language first
        Language language = new()
        {
            Name = "English",
            Culture = "en-US",
            Description = "English language",
            IsDeleted = false
        };
        await AddAsync(language);

        // Create a user to update
        AppUser userToUpdate = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };
        await AddAsync(userToUpdate);
        await AssignRoleToUserAsync(userToUpdate.Id, Roles.RegularUser.ToString());

        UpdateUserCommand command = new()
        {
            Id = userToUpdate.Id,
            Email = "invalid-email"
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenEmailExceedsMaxLength()
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

        // Create a user to update
        AppUser userToUpdate = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };
        await AddAsync(userToUpdate);
        await AssignRoleToUserAsync(userToUpdate.Id, Roles.RegularUser.ToString());

        UpdateUserCommand command = new()
        {
            Id = userToUpdate.Id,
            Email = new string('a', 248) + "@test.com" // 248 + 9 = 257 characters (exceeds max of 256)
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

        // Create a user to update
        AppUser userToUpdate = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };
        await AddAsync(userToUpdate);
        await AssignRoleToUserAsync(userToUpdate.Id, Roles.RegularUser.ToString());

        UpdateUserCommand command = new()
        {
            Id = userToUpdate.Id,
            Password = "12345" // Too short (minimum is 6)
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenPasswordExceedsMaxLength()
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

        // Create a user to update
        AppUser userToUpdate = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };
        await AddAsync(userToUpdate);
        await AssignRoleToUserAsync(userToUpdate.Id, Roles.RegularUser.ToString());

        UpdateUserCommand command = new()
        {
            Id = userToUpdate.Id,
            Password = new string('a', 101) // Exceeds max length of 100
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenLanguageIdIsInvalid()
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

        // Create a user to update
        AppUser userToUpdate = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };
        await AddAsync(userToUpdate);
        await AssignRoleToUserAsync(userToUpdate.Id, Roles.RegularUser.ToString());

        UpdateUserCommand command = new()
        {
            Id = userToUpdate.Id,
            LanguageId = -1 // Invalid language ID
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
        await AssignRoleToUserAsync(existingUser.Id, Roles.RegularUser.ToString());

        // Create a user to update
        AppUser userToUpdate = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };
        await AddAsync(userToUpdate);
        await AssignRoleToUserAsync(userToUpdate.Id, Roles.RegularUser.ToString());

        UpdateUserCommand command = new()
        {
            Id = userToUpdate.Id,
            UserName = "existinguser" // Same username as existing user
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
        await AssignRoleToUserAsync(existingUser.Id, Roles.RegularUser.ToString());

        // Create a user to update
        AppUser userToUpdate = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };
        await AddAsync(userToUpdate);
        await AssignRoleToUserAsync(userToUpdate.Id, Roles.RegularUser.ToString());

        UpdateUserCommand command = new()
        {
            Id = userToUpdate.Id,
            Email = "existing@example.com" // Same email as existing user
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

        // Create a user to update
        AppUser userToUpdate = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };
        await AddAsync(userToUpdate);
        await AssignRoleToUserAsync(userToUpdate.Id, Roles.RegularUser.ToString());

        UpdateUserCommand command = new()
        {
            Id = userToUpdate.Id,
            FirstName = "Updated"
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

        // Create a user to update
        AppUser userToUpdate = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };
        await AddAsync(userToUpdate);
        await AssignRoleToUserAsync(userToUpdate.Id, Roles.RegularUser.ToString());

        UpdateUserCommand command = new()
        {
            Id = userToUpdate.Id,
            FirstName = "Updated"
        };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task ShouldSetLastModifiedFields_WhenUserIsUpdated()
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

        // Create a user to update
        AppUser userToUpdate = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };
        await AddAsync(userToUpdate);
        await AssignRoleToUserAsync(userToUpdate.Id, Roles.RegularUser.ToString());

        UpdateUserCommand command = new()
        {
            Id = userToUpdate.Id,
            FirstName = "UpdatedFirstName"
        };

        // Act
        string userId = await SendAsync(command);

        // Assert
        userId.Should().Be(userToUpdate.Id);

        AppUser? updatedUser = await FindAsync<AppUser>(userToUpdate.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.LastModifiedBy.Should().Be(adminUserId);
        updatedUser.LastModified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task ShouldNotUpdateLastModifiedFields_WhenNoChangesMade()
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

        // Create a user to update
        AppUser userToUpdate = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };
        await AddAsync(userToUpdate);
        await AssignRoleToUserAsync(userToUpdate.Id, Roles.RegularUser.ToString());

        UpdateUserCommand command = new()
        {
            Id = userToUpdate.Id,
            FirstName = "Test" // Same as current value
        };

        // Act
        string userId = await SendAsync(command);

        // Assert
        userId.Should().Be(userToUpdate.Id);

        AppUser? updatedUser = await FindAsync<AppUser>(userToUpdate.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.FirstName.Should().Be("Test");
        // LastModified fields should not be updated when no actual changes are made
        // Since the user was just created, LastModified should remain null
        updatedUser.LastModified.Should().BeNull();
        updatedUser.LastModifiedBy.Should().BeNull();
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

        // Create a user to update
        AppUser userToUpdate = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };
        await AddAsync(userToUpdate);
        await AssignRoleToUserAsync(userToUpdate.Id, Roles.RegularUser.ToString());

        string longFirstName = new string('a', 100); // Maximum length
        string longLastName = new string('a', 100); // Maximum length
        string longUserName = new string('a', 256); // Maximum length
        string longEmail = new string('a', 247) + "@test.com"; // 247 + 9 = 256 characters (maximum)

        UpdateUserCommand command = new()
        {
            Id = userToUpdate.Id,
            FirstName = longFirstName,
            LastName = longLastName,
            UserName = longUserName,
            Email = longEmail
        };

        // Act
        string userId = await SendAsync(command);

        // Assert
        userId.Should().Be(userToUpdate.Id);

        AppUser? updatedUser = await FindAsync<AppUser>(userToUpdate.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.FirstName.Should().Be(longFirstName);
        updatedUser.LastName.Should().Be(longLastName);
        updatedUser.UserName.Should().Be(longUserName);
        updatedUser.Email.Should().Be(longEmail);
    }

    [Fact]
    public async Task ShouldUpdateMultipleFields_WhenMultipleFieldsProvided()
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

        // Create a user to update
        AppUser userToUpdate = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };
        await AddAsync(userToUpdate);
        await AssignRoleToUserAsync(userToUpdate.Id, Roles.RegularUser.ToString());

        UpdateUserCommand command = new()
        {
            Id = userToUpdate.Id,
            FirstName = "UpdatedFirst",
            LastName = "UpdatedLast",
            UserName = "updateduser",
            Email = "updated@example.com",
            LanguageId = language.Id
        };

        // Act
        string userId = await SendAsync(command);

        // Assert
        userId.Should().Be(userToUpdate.Id);

        AppUser? updatedUser = await FindAsync<AppUser>(userToUpdate.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.FirstName.Should().Be(command.FirstName);
        updatedUser.LastName.Should().Be(command.LastName);
        updatedUser.UserName.Should().Be(command.UserName);
        updatedUser.Email.Should().Be(command.Email);
        updatedUser.LanguageId.Should().Be(command.LanguageId);
    }
}
