using Application.Common.Behaviors;
using Domain.Entities.Identity;
using NUnit.Framework;

namespace Application.UnitTests.Common.Behaviors
{
    public sealed class FullNameResolverTests
    {
        [Test]
        public void Resolve_ShouldReturnEmptyString_WhenUserIsNull()
        {
            // Arrange
            AppUser? user = null;

            // Act
            string result = FullNameResolver.Resolve(user!);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Resolve_ShouldReturnEmail_WhenBothFirstNameAndLastNameAreEmpty()
        {
            // Arrange
            AppUser user = new()
            {
                FirstName = string.Empty,
                LastName = string.Empty,
                Email = "test@example.com"
            };

            // Act
            string result = FullNameResolver.Resolve(user);

            // Assert
            Assert.That(result, Is.EqualTo("test@example.com"));
        }

        [Test]
        public void Resolve_ShouldReturnEmail_WhenBothFirstNameAndLastNameAreNull()
        {
            // Arrange
            AppUser user = new()
            {
                FirstName = null!,
                LastName = null!,
                Email = "test@example.com"
            };

            // Act
            string result = FullNameResolver.Resolve(user);

            // Assert
            Assert.That(result, Is.EqualTo("test@example.com"));
        }

        [Test]
        public void Resolve_ShouldReturnConcatenatedWhitespace_WhenBothFirstNameAndLastNameAreWhitespace()
        {
            // Arrange
            AppUser user = new()
            {
                FirstName = "   ",
                LastName = "   ",
                Email = "test@example.com"
            };

            // Act
            string result = FullNameResolver.Resolve(user);

            // Assert
            // string.IsNullOrEmpty("   ") returns false, so it concatenates the names
            Assert.That(result, Is.EqualTo("       "));
        }

        [Test]
        public void Resolve_ShouldReturnFullName_WhenBothFirstNameAndLastNameAreProvided()
        {
            // Arrange
            AppUser user = new()
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            // Act
            string result = FullNameResolver.Resolve(user);

            // Assert
            Assert.That(result, Is.EqualTo("John Doe"));
        }

        [Test]
        public void Resolve_ShouldReturnFirstNameOnly_WhenFirstNameIsProvidedButLastNameIsEmpty()
        {
            // Arrange
            AppUser user = new()
            {
                FirstName = "John",
                LastName = string.Empty,
                Email = "john@example.com"
            };

            // Act
            string result = FullNameResolver.Resolve(user);

            // Assert
            Assert.That(result, Is.EqualTo("John"));
        }

        [Test]
        public void Resolve_ShouldReturnFirstNameOnly_WhenFirstNameIsProvidedButLastNameIsNull()
        {
            // Arrange
            AppUser user = new()
            {
                FirstName = "John",
                LastName = null!,
                Email = "john@example.com"
            };

            // Act
            string result = FullNameResolver.Resolve(user);

            // Assert
            Assert.That(result, Is.EqualTo("John"));
        }

        [Test]
        public void Resolve_ShouldReturnConcatenatedName_WhenFirstNameIsProvidedButLastNameIsWhitespace()
        {
            // Arrange
            AppUser user = new()
            {
                FirstName = "John",
                LastName = "   ",
                Email = "john@example.com"
            };

            // Act
            string result = FullNameResolver.Resolve(user);

            // Assert
            // string.IsNullOrEmpty("   ") returns false, so it concatenates the names
            Assert.That(result, Is.EqualTo("John    "));
        }

        [Test]
        public void Resolve_ShouldReturnLastNameOnly_WhenLastNameIsProvidedButFirstNameIsEmpty()
        {
            // Arrange
            AppUser user = new()
            {
                FirstName = string.Empty,
                LastName = "Doe",
                Email = "doe@example.com"
            };

            // Act
            string result = FullNameResolver.Resolve(user);

            // Assert
            Assert.That(result, Is.EqualTo("Doe"));
        }

        [Test]
        public void Resolve_ShouldReturnLastNameOnly_WhenLastNameIsProvidedButFirstNameIsNull()
        {
            // Arrange
            AppUser user = new()
            {
                FirstName = null!,
                LastName = "Doe",
                Email = "doe@example.com"
            };

            // Act
            string result = FullNameResolver.Resolve(user);

            // Assert
            Assert.That(result, Is.EqualTo("Doe"));
        }

        [Test]
        public void Resolve_ShouldReturnConcatenatedName_WhenLastNameIsProvidedButFirstNameIsWhitespace()
        {
            // Arrange
            AppUser user = new()
            {
                FirstName = "   ",
                LastName = "Doe",
                Email = "doe@example.com"
            };

            // Act
            string result = FullNameResolver.Resolve(user);

            // Assert
            // string.IsNullOrEmpty("   ") returns false, so it concatenates the names
            Assert.That(result, Is.EqualTo("    Doe"));
        }

        [Test]
        public void Resolve_ShouldHandleNamesWithSpecialCharacters_WhenBothNamesAreProvided()
        {
            // Arrange
            AppUser user = new()
            {
                FirstName = "José-María",
                LastName = "O'Connor-Smith",
                Email = "jose@example.com"
            };

            // Act
            string result = FullNameResolver.Resolve(user);

            // Assert
            Assert.That(result, Is.EqualTo("José-María O'Connor-Smith"));
        }

        [Test]
        public void Resolve_ShouldHandleNamesWithNumbers_WhenBothNamesAreProvided()
        {
            // Arrange
            AppUser user = new()
            {
                FirstName = "John2",
                LastName = "Doe3",
                Email = "john2@example.com"
            };

            // Act
            string result = FullNameResolver.Resolve(user);

            // Assert
            Assert.That(result, Is.EqualTo("John2 Doe3"));
        }

        [Test]
        public void Resolve_ShouldHandleVeryLongNames_WhenBothNamesAreProvided()
        {
            // Arrange
            string longFirstName = new string('A', 100);
            string longLastName = new string('B', 100);
            AppUser user = new()
            {
                FirstName = longFirstName,
                LastName = longLastName,
                Email = "longname@example.com"
            };

            // Act
            string result = FullNameResolver.Resolve(user);

            // Assert
            Assert.That(result, Is.EqualTo($"{longFirstName} {longLastName}"));
        }

        [Test]
        public void Resolve_ShouldHandleUnicodeNames_WhenBothNamesAreProvided()
        {
            // Arrange
            AppUser user = new()
            {
                FirstName = "李",
                LastName = "明",
                Email = "liming@example.com"
            };

            // Act
            string result = FullNameResolver.Resolve(user);

            // Assert
            Assert.That(result, Is.EqualTo("李 明"));
        }

        [Test]
        public void Resolve_ShouldHandleEmptyEmail_WhenBothNamesAreEmpty()
        {
            // Arrange
            AppUser user = new()
            {
                FirstName = string.Empty,
                LastName = string.Empty,
                Email = string.Empty
            };

            // Act
            string result = FullNameResolver.Resolve(user);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Resolve_ShouldReturnNull_WhenBothNamesAreEmptyAndEmailIsNull()
        {
            // Arrange
            AppUser user = new()
            {
                FirstName = string.Empty,
                LastName = string.Empty,
                Email = null!
            };

            // Act
            string result = FullNameResolver.Resolve(user);

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}
