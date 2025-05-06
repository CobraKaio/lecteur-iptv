using System;
using System.Linq;
using System.Threading.Tasks;
using LecteurIptv.Backend.Data;
using LecteurIptv.Backend.Models;
using LecteurIptv.Backend.Services;
using LecteurIptv.Backend.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LecteurIptv.Backend.Tests.Services
{
    public class UserServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly UserService _userService;
        private readonly string _dbName;

        public UserServiceTests()
        {
            // Create a unique database name for each test run to ensure isolation
            _dbName = $"UserServiceTests_{Guid.NewGuid()}";
            _context = TestDbContextFactory.CreateDbContext(_dbName);

            // Set up mocks
            _loggerMock = new Mock<ILogger<UserService>>();

            // Create the service with mocked dependencies
            _userService = new UserService(_loggerMock.Object, _context);

            // Seed the database with test data
            TestDataGenerator.SeedUsers(_context);
        }

        public void Dispose()
        {
            // Clean up after each test
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsAllUsers()
        {
            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetUserByIdAsync_WithValidId_ReturnsUser()
        {
            // Arrange
            var userId = 1;

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var userId = 999; // Non-existent ID

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_WithValidUsername_ReturnsUser()
        {
            // Arrange
            var username = "testuser";

            // Act
            var result = await _userService.GetUserByUsernameAsync(username);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(username, result.Username);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_WithInvalidUsername_ReturnsNull()
        {
            // Arrange
            var username = "nonexistentuser"; // Non-existent username

            // Act
            var result = await _userService.GetUserByUsernameAsync(username);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByEmailAsync_WithValidEmail_ReturnsUser()
        {
            // Arrange
            var email = "testuser@example.com";

            // Act
            var result = await _userService.GetUserByEmailAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public async Task GetUserByEmailAsync_WithInvalidEmail_ReturnsNull()
        {
            // Arrange
            var email = "nonexistent@example.com"; // Non-existent email

            // Act
            var result = await _userService.GetUserByEmailAsync(email);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_WithValidCredentials_ReturnsUser()
        {
            // Arrange
            var username = "testuser";
            var password = "password";

            // Act
            var result = await _userService.AuthenticateAsync(username, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(username, result.Username);
        }

        [Fact]
        public async Task AuthenticateAsync_WithInvalidUsername_ReturnsNull()
        {
            // Arrange
            var username = "nonexistentuser"; // Non-existent username
            var password = "password";

            // Act
            var result = await _userService.AuthenticateAsync(username, password);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_WithInvalidPassword_ReturnsNull()
        {
            // Arrange
            var username = "testuser";
            var password = "wrongpassword"; // Wrong password

            // Act
            var result = await _userService.AuthenticateAsync(username, password);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateUserAsync_WithValidData_CreatesUser()
        {
            // Arrange
            var newUser = new User
            {
                Username = "newuser",
                Email = "newuser@example.com",
                FirstName = "New",
                LastName = "User",
                Role = "User"
            };
            var password = "newpassword";

            // Act
            var result = await _userService.CreateUserAsync(newUser, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newUser.Username, result.Username);
            Assert.Equal(newUser.Email, result.Email);

            // Verify it was added to the database
            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Username == "newuser");
            Assert.NotNull(userInDb);
            Assert.True(userInDb.IsActive);
        }

        [Fact]
        public async Task CreateUserAsync_WithExistingUsername_ThrowsArgumentException()
        {
            // Arrange
            var newUser = new User
            {
                Username = "testuser", // Existing username
                Email = "newuser@example.com",
                FirstName = "New",
                LastName = "User",
                Role = "User"
            };
            var password = "newpassword";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _userService.CreateUserAsync(newUser, password));
        }

        [Fact]
        public async Task CreateUserAsync_WithExistingEmail_ThrowsArgumentException()
        {
            // Arrange
            var newUser = new User
            {
                Username = "newuser",
                Email = "testuser@example.com", // Existing email
                FirstName = "New",
                LastName = "User",
                Role = "User"
            };
            var password = "newpassword";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _userService.CreateUserAsync(newUser, password));
        }

        [Fact]
        public async Task CreateUserAsync_WithEmptyPassword_ThrowsArgumentException()
        {
            // Arrange
            var newUser = new User
            {
                Username = "newuser",
                Email = "newuser@example.com",
                FirstName = "New",
                LastName = "User",
                Role = "User"
            };
            var password = ""; // Empty password

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _userService.CreateUserAsync(newUser, password));
        }

        [Fact]
        public async Task UpdateUserAsync_WithValidData_UpdatesUser()
        {
            // Arrange
            var userId = 1;
            var updatedUser = new User
            {
                Id = userId,
                Username = "testuser", // Same username to avoid unique constraint
                Email = "testuser@example.com", // Same email to avoid unique constraint
                FirstName = "Updated",
                LastName = "User",
                Role = "User"
            };

            // Act
            var result = await _userService.UpdateUserAsync(userId, updatedUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated", result.FirstName);

            // Verify it was updated in the database
            var userInDb = await _context.Users.FindAsync(userId);
            Assert.Equal("Updated", userInDb.FirstName);
        }

        [Fact]
        public async Task UpdateUserAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var userId = 999; // Non-existent ID
            var updatedUser = new User
            {
                Id = userId,
                Username = "updateduser",
                Email = "updated@example.com",
                FirstName = "Updated",
                LastName = "User",
                Role = "User"
            };

            // Act
            var result = await _userService.UpdateUserAsync(userId, updatedUser);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdatePasswordAsync_WithValidData_UpdatesPassword()
        {
            // Arrange
            var userId = 1;
            var currentPassword = "password";
            var newPassword = "newpassword";

            // Act
            var result = await _userService.UpdatePasswordAsync(userId, currentPassword, newPassword);

            // Assert
            Assert.True(result);

            // Verify the password was updated by trying to authenticate
            var user = await _userService.GetUserByIdAsync(userId);
            var authResult = await _userService.AuthenticateAsync(user.Username, newPassword);
            Assert.NotNull(authResult);
        }

        [Fact]
        public async Task UpdatePasswordAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var userId = 999; // Non-existent ID
            var currentPassword = "password";
            var newPassword = "newpassword";

            // Act
            var result = await _userService.UpdatePasswordAsync(userId, currentPassword, newPassword);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdatePasswordAsync_WithInvalidCurrentPassword_ReturnsFalse()
        {
            // Arrange
            var userId = 1;
            var currentPassword = "wrongpassword"; // Wrong current password
            var newPassword = "newpassword";

            // Act
            var result = await _userService.UpdatePasswordAsync(userId, currentPassword, newPassword);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteUserAsync_WithValidId_DeletesUser()
        {
            // Arrange
            var userId = 1;

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            Assert.True(result);

            // Verify it was deleted from the database
            var userInDb = await _context.Users.FindAsync(userId);
            Assert.Null(userInDb);
        }

        [Fact]
        public async Task DeleteUserAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var userId = 999; // Non-existent ID

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UsernameExistsAsync_WithExistingUsername_ReturnsTrue()
        {
            // Arrange
            var username = "testuser";

            // Act
            var result = await _userService.UsernameExistsAsync(username);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UsernameExistsAsync_WithNonExistingUsername_ReturnsFalse()
        {
            // Arrange
            var username = "nonexistentuser";

            // Act
            var result = await _userService.UsernameExistsAsync(username);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task EmailExistsAsync_WithExistingEmail_ReturnsTrue()
        {
            // Arrange
            var email = "testuser@example.com";

            // Act
            var result = await _userService.EmailExistsAsync(email);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task EmailExistsAsync_WithNonExistingEmail_ReturnsFalse()
        {
            // Arrange
            var email = "nonexistent@example.com";

            // Act
            var result = await _userService.EmailExistsAsync(email);

            // Assert
            Assert.False(result);
        }
    }
}
