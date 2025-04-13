using ContactList.Web.Models;
using ContactList.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;

namespace ContactList.Tests.Services
{
    [TestFixture]
    public class AuthServiceTests
    {
        private IAuthService _authService;
        private AppDbContext _context;
        private Mock<ILogger<AuthService>> _mockLogger;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _mockLogger = new Mock<ILogger<AuthService>>();
            _authService = new AuthService(_context, _mockLogger.Object);
        }

        [Test]
        public void Register_ValidUser_ReturnsTrue()
        {
            // Arrange
            var username = "testuser";
            var password = "Test@123";

            // Act
            var result = _authService.Register(username, password);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(_context.Users.FirstOrDefault(u => u.Username == username));
        }

        [Test]
        public void Register_DuplicateUsername_ReturnsFalse()
        {
            // Arrange
            var username = "existinguser";
            _authService.Register(username, "Test@123");

            // Act
            var result = _authService.Register(username, "AnotherPass");

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Authenticate_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var username = "loginuser";
            var password = "Login@123";
            _authService.Register(username, password);

            // Act
            var user = _authService.Authenticate(username, password);

            // Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(username, user.Username);
        }

        [Test]
        public void Authenticate_InvalidCredentials_ReturnsNull()
        {
            // Arrange
            var username = "invaliduser";
            var password = "wrongpassword";

            // Act
            var result = _authService.Authenticate(username, password);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void ResetPassword_ValidUser_ChangesPassword()
        {
            // Arrange
            var username = "changepassuser";
            var oldPassword = "Old@123";
            var newPassword = "New@456";
            _authService.Register(username, oldPassword);

            // Act
            var result = _authService.ResetPassword(username, newPassword);
            var user = _authService.Authenticate(username, newPassword);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(user);
        }

        [Test]
        public void ResetPassword_NonExistentUser_ReturnsFalse()
        {
            // Act
            var result = _authService.ResetPassword("nouser", "SomePassword");

            // Assert
            Assert.IsFalse(result);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
