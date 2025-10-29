using Application.Exceptions;
using Application.Users.Queries.GetById;
using Domain.Entities;
using Domain.Entities.Identity;
using Domain.Enums;
using FluentAssertions;

namespace Application.IntegrationTests.Users.Queries;

using static Testing;

public class GetUserByIdQueryTests : BaseTestFixture
{
    [Fact]
    public async Task ShouldReturnUserById_WhenUserExists()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();

        // Create a language first
        Language language = new Language
        {
            Name = "English",
            Culture = "en-US",
            Description = "English language",
            IsDeleted = false
        };
        await AddAsync(language);

        // Create a user
        AppUser user = new AppUser
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "+1234567890",
            LanguageId = language.Id,
            IsDeleted = false
        };

        await AddAsync(user);

        // Create admin role and assign to user
        await AssignRoleToUserAsync(user.Id, Roles.Admin.ToString());

        GetUserByIdQuery query = new GetUserByIdQuery { Id = user.Id };

        // Act
        GetUserByIdDto result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
        result.FirstName.Should().Be(user.FirstName);
        result.LastName.Should().Be(user.LastName);
        result.FullName.Should().Be($"{user.FirstName} {user.LastName}");
        result.UserName.Should().Be(user.UserName);
        result.PhoneNumber.Should().Be(user.PhoneNumber);
        result.IsDeleted.Should().Be(user.IsDeleted);
        result.Role.Should().Be(Roles.Admin.ToString());
        result.Language.Should().Be(language.Name);
        result.Culture.Should().Be(language.Culture);
    }

    [Fact]
    public async Task ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();

        GetUserByIdQuery query = new GetUserByIdQuery { Id = "non-existent-id" };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(query);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"GetUserByIdDto\" by (non-existent-id) was not found.");
    }

    [Fact]
    public async Task ShouldThrowNotFoundException_WhenUserIsDeleted()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();

        // Create a language first
        Language language = new Language
        {
            Name = "English",
            Culture = "en-US",
            Description = "English language",
            IsDeleted = false
        };
        await AddAsync(language);

        // Create a deleted user
        AppUser user = new AppUser
        {
            UserName = "deleteduser",
            Email = "deleted@example.com",
            FirstName = "Deleted",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = true
        };

        await AddAsync(user);

        GetUserByIdQuery query = new GetUserByIdQuery { Id = user.Id };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(query);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"GetUserByIdDto\" by ({user.Id}) was not found.");
    }

    [Fact]
    public async Task ShouldReturnUserWithNullLanguage_WhenLanguageIsNotSet()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();

        // Create a user without language
        AppUser user = new AppUser
        {
            UserName = "userwithoutlang",
            Email = "nolang@example.com",
            FirstName = "No",
            LastName = "Language",
            LanguageId = null,
            IsDeleted = false
        };

        await AddAsync(user);

        // Create admin role and assign to user
        await AssignRoleToUserAsync(user.Id, Roles.Admin.ToString());

        GetUserByIdQuery query = new GetUserByIdQuery { Id = user.Id };

        // Act
        GetUserByIdDto result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
        result.FirstName.Should().Be(user.FirstName);
        result.LastName.Should().Be(user.LastName);
        result.FullName.Should().Be($"{user.FirstName} {user.LastName}");
        result.UserName.Should().Be(user.UserName);
        result.IsDeleted.Should().Be(user.IsDeleted);
        result.Role.Should().Be(Roles.Admin.ToString());
        result.Language.Should().BeNull();
        result.Culture.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReturnUserWithMultipleRoles_WhenUserHasMultipleRoles()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();

        // Create a language first
        Language language = new Language
        {
            Name = "English",
            Culture = "en-US",
            Description = "English language",
            IsDeleted = false
        };
        await AddAsync(language);

        // Create a user
        AppUser user = new AppUser
        {
            UserName = "multiroleuser",
            Email = "multi@example.com",
            FirstName = "Multi",
            LastName = "Role",
            LanguageId = language.Id,
            IsDeleted = false
        };

        await AddAsync(user);

        // Create roles and assign to user
        await AssignRoleToUserAsync(user.Id, Roles.Admin.ToString());
        await AssignRoleToUserAsync(user.Id, Roles.RegularUser.ToString());

        GetUserByIdQuery query = new GetUserByIdQuery { Id = user.Id };

        // Act
        GetUserByIdDto result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.Role.Should().BeOneOf(Roles.Admin.ToString(), Roles.RegularUser.ToString()); // Should return the first role
    }

    [Fact]
    public async Task ShouldReturnUserWithEmptyPhoneNumber_WhenPhoneNumberIsNull()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();

        // Create a language first
        Language language = new Language
        {
            Name = "English",
            Culture = "en-US",
            Description = "English language",
            IsDeleted = false
        };
        await AddAsync(language);

        // Create a user without phone number
        AppUser user = new AppUser
        {
            UserName = "nophoneuser",
            Email = "nophone@example.com",
            FirstName = "No",
            LastName = "Phone",
            PhoneNumber = null,
            LanguageId = language.Id,
            IsDeleted = false
        };

        await AddAsync(user);

        // Create admin role and assign to user
        await AssignRoleToUserAsync(user.Id, Roles.Admin.ToString());

        GetUserByIdQuery query = new GetUserByIdQuery { Id = user.Id };

        // Act
        GetUserByIdDto result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.PhoneNumber.Should().BeNull();
    }

    [Fact]
    public async Task ShouldHandleEmptyUserId_WhenUserIdIsEmpty()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();

        GetUserByIdQuery query = new GetUserByIdQuery { Id = string.Empty };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(query);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"GetUserByIdDto\" by () was not found.");
    }

    [Fact]
    public async Task ShouldHandleWhitespaceUserId_WhenUserIdIsWhitespace()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();

        GetUserByIdQuery query = new GetUserByIdQuery { Id = "   " };

        // Act & Assert
        Func<Task> act = async () => await SendAsync(query);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"GetUserByIdDto\" by (   ) was not found.");
    }

    [Fact]
    public async Task ShouldReturnUserWithSpecialCharacters_WhenUserHasSpecialCharactersInName()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();

        // Create a language first
        Language language = new Language
        {
            Name = "English",
            Culture = "en-US",
            Description = "English language",
            IsDeleted = false
        };
        await AddAsync(language);

        // Create a user with special characters
        AppUser user = new AppUser
        {
            UserName = "special@user",
            Email = "special@example.com",
            FirstName = "José",
            LastName = "García-López",
            LanguageId = language.Id,
            IsDeleted = false
        };

        await AddAsync(user);

        // Create admin role and assign to user
        await AssignRoleToUserAsync(user.Id, Roles.Admin.ToString());

        GetUserByIdQuery query = new GetUserByIdQuery { Id = user.Id };

        // Act
        GetUserByIdDto result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.FirstName.Should().Be(user.FirstName);
        result.LastName.Should().Be(user.LastName);
        result.FullName.Should().Be($"{user.FirstName} {user.LastName}");
    }
}