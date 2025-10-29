using Application.Exceptions;
using Application.Users.Queries.GetAll;
using Application.Common.Pagination.Models;
using Domain.Entities;
using Domain.Entities.Identity;
using Domain.Enums;
using FluentAssertions;

namespace Application.IntegrationTests.Users.Queries;

using static Testing;

[Collection("Integration Tests")]
public class GetAllUsersQueryTests : BaseTestFixture
{
    [Fact]
    public async Task ShouldReturnAllUsers_WhenUsersExist()
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

        // Create multiple users
        AppUser user1 = new()
        {
            UserName = "user1",
            Email = "user1@example.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "+1234567890",
            LanguageId = language.Id,
            IsDeleted = false
        };

        AppUser user2 = new()
        {
            UserName = "user2",
            Email = "user2@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            PhoneNumber = "+0987654321",
            LanguageId = language.Id,
            IsDeleted = false
        };

        await AddAsync(user1);
        await AddAsync(user2);

        // Assign roles to users
        await AssignRoleToUserAsync(user1.Id, Roles.RegularUser.ToString());
        await AssignRoleToUserAsync(user2.Id, Roles.RegularUser.ToString());

        GetAllUsersQuery query = new(1, 10);

        // Act
        PaginationResultDto<GetAllUsersDto> result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.List.Should().HaveCount(2);
        result.PageNumber.Should().Be(1);
        result.TotalCount.Should().Be(2);
        result.TotalPages.Should().Be(1);

        var user1Result = result.List.FirstOrDefault(u => u.Id == user1.Id);
        user1Result.Should().NotBeNull();
        user1Result!.Email.Should().Be(user1.Email);
        user1Result.FullName.Should().Be($"{user1.FirstName} {user1.LastName}");
        user1Result.UserName.Should().Be(user1.UserName);
        user1Result.PhoneNumber.Should().Be(user1.PhoneNumber);
        user1Result.Role.Should().Be(Roles.RegularUser.ToString());

        var user2Result = result.List.FirstOrDefault(u => u.Id == user2.Id);
        user2Result.Should().NotBeNull();
        user2Result!.Email.Should().Be(user2.Email);
        user2Result.FullName.Should().Be($"{user2.FirstName} {user2.LastName}");
        user2Result.UserName.Should().Be(user2.UserName);
        user2Result.PhoneNumber.Should().Be(user2.PhoneNumber);
        user2Result.Role.Should().Be(Roles.RegularUser.ToString());
    }

    [Fact]
    public async Task ShouldExcludeCurrentUser_FromResults()
    {
        // Arrange
        await ResetState();
        string currentUserId = await RunAsAdministratorAsync();

        // Create a language first
        Language language = new()
        {
            Name = "English",
            Culture = "en-US",
            Description = "English language",
            IsDeleted = false
        };
        await AddAsync(language);

        // Create another user
        AppUser otherUser = new()
        {
            UserName = "otheruser",
            Email = "other@example.com",
            FirstName = "Other",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };

        await AddAsync(otherUser);
        await AssignRoleToUserAsync(otherUser.Id, Roles.RegularUser.ToString());

        GetAllUsersQuery query = new(1, 10);

        // Act
        PaginationResultDto<GetAllUsersDto> result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.List.Should().HaveCount(1);
        result.List.Should().NotContain(u => u.Id == currentUserId);
        result.List.Should().Contain(u => u.Id == otherUser.Id);
    }

    [Fact]
    public async Task ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();

        GetAllUsersQuery query = new(1, 10);

        // Act
        PaginationResultDto<GetAllUsersDto> result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.List.Should().BeEmpty();
        result.PageNumber.Should().Be(1);
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task ShouldFilterUsersBySearchTerm_WhenTermIsProvided()
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

        // Create users with different names
        AppUser johnUser = new()
        {
            UserName = "johnuser",
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Doe",
            LanguageId = language.Id,
            IsDeleted = false
        };

        AppUser janeUser = new()
        {
            UserName = "janeuser",
            Email = "jane@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            LanguageId = language.Id,
            IsDeleted = false
        };

        AppUser bobUser = new()
        {
            UserName = "bobuser",
            Email = "bob@example.com",
            FirstName = "Bob",
            LastName = "Johnson",
            LanguageId = language.Id,
            IsDeleted = false
        };

        await AddAsync(johnUser);
        await AddAsync(janeUser);
        await AddAsync(bobUser);

        await AssignRoleToUserAsync(johnUser.Id, Roles.RegularUser.ToString());
        await AssignRoleToUserAsync(janeUser.Id, Roles.RegularUser.ToString());
        await AssignRoleToUserAsync(bobUser.Id, Roles.RegularUser.ToString());

        GetAllUsersQuery query = new(1, 10, "Doe");

        // Act
        PaginationResultDto<GetAllUsersDto> result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.List.Should().HaveCount(1);
        result.List.Should().Contain(u => u.Id == johnUser.Id);
        result.List.Should().NotContain(u => u.Id == janeUser.Id);
        result.List.Should().NotContain(u => u.Id == bobUser.Id);
    }

    [Fact]
    public async Task ShouldSearchByLastName_WhenTermMatchesLastName()
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

        // Create users
        AppUser smithUser = new()
        {
            UserName = "smithuser",
            Email = "smith@example.com",
            FirstName = "John",
            LastName = "Smith",
            LanguageId = language.Id,
            IsDeleted = false
        };

        AppUser doeUser = new()
        {
            UserName = "doeuser",
            Email = "doe@example.com",
            FirstName = "Jane",
            LastName = "Doe",
            LanguageId = language.Id,
            IsDeleted = false
        };

        await AddAsync(smithUser);
        await AddAsync(doeUser);

        await AssignRoleToUserAsync(smithUser.Id, Roles.RegularUser.ToString());
        await AssignRoleToUserAsync(doeUser.Id, Roles.RegularUser.ToString());

        GetAllUsersQuery query = new(1, 10, "Smith");

        // Act
        PaginationResultDto<GetAllUsersDto> result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.List.Should().HaveCount(1);
        result.List.Should().Contain(u => u.Id == smithUser.Id);
        result.List.Should().NotContain(u => u.Id == doeUser.Id);
    }

    [Fact]
    public async Task ShouldSearchByUserName_WhenTermMatchesUserName()
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

        // Create users
        AppUser testUser = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };

        AppUser demoUser = new()
        {
            UserName = "demouser",
            Email = "demo@example.com",
            FirstName = "Demo",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };

        await AddAsync(testUser);
        await AddAsync(demoUser);

        await AssignRoleToUserAsync(testUser.Id, Roles.RegularUser.ToString());
        await AssignRoleToUserAsync(demoUser.Id, Roles.RegularUser.ToString());

        GetAllUsersQuery query = new(1, 10, "test");

        // Act
        PaginationResultDto<GetAllUsersDto> result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.List.Should().HaveCount(1);
        result.List.Should().Contain(u => u.Id == testUser.Id);
        result.List.Should().NotContain(u => u.Id == demoUser.Id);
    }

    [Fact]
    public async Task ShouldReturnCorrectPagination_WhenMultiplePagesExist()
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

        // Create 5 users
        for (int i = 1; i <= 5; i++)
        {
            AppUser user = new()
            {
                UserName = $"user{i}",
                Email = $"user{i}@example.com",
                FirstName = $"User{i}",
                LastName = $"LastName{i}",
                LanguageId = language.Id,
                IsDeleted = false
            };

            await AddAsync(user);
            await AssignRoleToUserAsync(user.Id, Roles.RegularUser.ToString());
        }

        GetAllUsersQuery query = new(1, 2); // Page 1, size 2

        // Act
        PaginationResultDto<GetAllUsersDto> result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.List.Should().HaveCount(2);
        result.PageNumber.Should().Be(1);
        result.TotalCount.Should().Be(5);
        result.TotalPages.Should().Be(3); // 5 users / 2 per page = 3 pages
    }

    [Fact]
    public async Task ShouldReturnCorrectPage_WhenRequestingSecondPage()
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

        // Create 5 users
        for (int i = 1; i <= 5; i++)
        {
            AppUser user = new()
            {
                UserName = $"user{i}",
                Email = $"user{i}@example.com",
                FirstName = $"User{i}",
                LastName = $"LastName{i}",
                LanguageId = language.Id,
                IsDeleted = false
            };

            await AddAsync(user);
            await AssignRoleToUserAsync(user.Id, Roles.RegularUser.ToString());
        }

        GetAllUsersQuery query = new(2, 2); // Page 2, size 2

        // Act
        PaginationResultDto<GetAllUsersDto> result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.List.Should().HaveCount(2);
        result.PageNumber.Should().Be(2);
        result.TotalCount.Should().Be(5);
        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenPageNumberIsInvalid()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();

        GetAllUsersQuery query = new(0, 10); // Invalid page number

        // Act & Assert
        Func<Task> act = async () => await SendAsync(query);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenPageSizeIsInvalid()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();

        GetAllUsersQuery query = new(1, 0); // Invalid page size

        // Act & Assert
        Func<Task> act = async () => await SendAsync(query);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenPageSizeExceedsMaximum()
    {
        // Arrange
        await ResetState();
        await RunAsAdministratorAsync();

        GetAllUsersQuery query = new(1, 101); // Page size exceeds maximum

        // Act & Assert
        Func<Task> act = async () => await SendAsync(query);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ShouldHandleLongSearchTerm_WhenSearchTermIsVeryLong()
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

        // Create a user
        AppUser testUser = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };

        await AddAsync(testUser);
        await AssignRoleToUserAsync(testUser.Id, Roles.RegularUser.ToString());

        string longSearchTerm = new('a', 101); // Exceeds 100 character limit
        GetAllUsersQuery query = new(1, 10, longSearchTerm);

        // Act
        PaginationResultDto<GetAllUsersDto> result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.List.Should().BeEmpty(); // No matches for such a long term
        result.PageNumber.Should().Be(1);
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task ShouldReturnUsersOrderedByUserName()
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

        // Create users with different usernames
        AppUser charlieUser = new()
        {
            UserName = "charlie",
            Email = "charlie@example.com",
            FirstName = "Charlie",
            LastName = "Brown",
            LanguageId = language.Id,
            IsDeleted = false
        };

        AppUser aliceUser = new()
        {
            UserName = "alice",
            Email = "alice@example.com",
            FirstName = "Alice",
            LastName = "Wonder",
            LanguageId = language.Id,
            IsDeleted = false
        };

        AppUser bobUser = new()
        {
            UserName = "bob",
            Email = "bob@example.com",
            FirstName = "Bob",
            LastName = "Builder",
            LanguageId = language.Id,
            IsDeleted = false
        };

        await AddAsync(charlieUser);
        await AddAsync(aliceUser);
        await AddAsync(bobUser);

        await AssignRoleToUserAsync(charlieUser.Id, Roles.RegularUser.ToString());
        await AssignRoleToUserAsync(aliceUser.Id, Roles.RegularUser.ToString());
        await AssignRoleToUserAsync(bobUser.Id, Roles.RegularUser.ToString());

        GetAllUsersQuery query = new(1, 10);

        // Act
        PaginationResultDto<GetAllUsersDto> result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.List.Should().HaveCount(3);
        result.List[0].UserName.Should().Be("alice");
        result.List[1].UserName.Should().Be("bob");
        result.List[2].UserName.Should().Be("charlie");
    }

    [Fact]
    public async Task ShouldHandleUsersWithEmptyUserName()
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

        // Create a user with empty username
        AppUser emptyUserNameUser = new()
        {
            UserName = string.Empty,
            Email = "emptyuser@example.com",
            FirstName = "Empty",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };

        await AddAsync(emptyUserNameUser);
        await AssignRoleToUserAsync(emptyUserNameUser.Id, Roles.RegularUser.ToString());

        GetAllUsersQuery query = new(1, 10);

        // Act
        PaginationResultDto<GetAllUsersDto> result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.List.Should().HaveCount(1);
        result.List[0].Id.Should().Be(emptyUserNameUser.Id);
        result.List[0].UserName.Should().Be(string.Empty);
    }

    [Fact]
    public async Task ShouldHandleUsersWithNullPhoneNumber()
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

        // Create a user without phone number
        AppUser noPhoneUser = new()
        {
            UserName = "nophone",
            Email = "nophone@example.com",
            FirstName = "No",
            LastName = "Phone",
            PhoneNumber = null,
            LanguageId = language.Id,
            IsDeleted = false
        };

        await AddAsync(noPhoneUser);
        await AssignRoleToUserAsync(noPhoneUser.Id, Roles.RegularUser.ToString());

        GetAllUsersQuery query = new(1, 10);

        // Act
        PaginationResultDto<GetAllUsersDto> result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.List.Should().HaveCount(1);
        result.List[0].Id.Should().Be(noPhoneUser.Id);
        result.List[0].PhoneNumber.Should().BeNull();
    }

    [Fact]
    public async Task ShouldHandleCaseInsensitiveSearch()
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

        // Create a user
        AppUser testUser = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };

        await AddAsync(testUser);
        await AssignRoleToUserAsync(testUser.Id, Roles.RegularUser.ToString());

        GetAllUsersQuery query = new(1, 10, "TEST"); // Uppercase search term

        // Act
        PaginationResultDto<GetAllUsersDto> result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.List.Should().HaveCount(1);
        result.List.Should().Contain(u => u.Id == testUser.Id);
    }

    [Fact]
    public async Task ShouldReturnEmptyResults_WhenSearchTermDoesNotMatch()
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

        // Create a user
        AppUser testUser = new()
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            LanguageId = language.Id,
            IsDeleted = false
        };

        await AddAsync(testUser);
        await AssignRoleToUserAsync(testUser.Id, Roles.RegularUser.ToString());

        GetAllUsersQuery query = new(1, 10, "nonexistent");

        // Act
        PaginationResultDto<GetAllUsersDto> result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.List.Should().BeEmpty();
        result.PageNumber.Should().Be(1);
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }
}