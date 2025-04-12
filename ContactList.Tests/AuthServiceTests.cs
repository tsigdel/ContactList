using ContactList.Web.Models;
using ContactList.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace ContactList.Tests.Services
{
    [TestFixture]
    public class AuthServiceTests
    {
        private AuthService _authService;
        private AppDbContext _context;
        private Mock<ILogger<AuthService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "AuthDb")
                .Options;

            _context = new AppDbContext(options);
            _context.Database.EnsureDeleted(); // Clean DB before each test
            _context.Database.EnsureCreated();

            _loggerMock = new Mock<ILogger<AuthService>>();
            _authService = new AuthService(_context, _loggerMock.Object);
        }

        [Test]
        public void Register_NewUser_ReturnsTrue()
        {
            var result = _authService.Register("testuser", "password123");
            Assert.IsTrue(result);
        }

        [Test]
        public void Register_ExistingUsername_ReturnsFalse()
        {
            _authService.Register("testuser", "password123");
            var result = _authService.Register("testuser", "newpass");
            Assert.IsFalse(result);
        }

        [Test]
        public void Authenticate_ValidCredentials_ReturnsUser()
        {
            _authService.Register("testuser", "password123");
            var user = _authService.Authenticate("testuser", "password123");

            Assert.IsNotNull(user);
            Assert.AreEqual("testuser", user.Username);
        }

        [Test]
        public void Authenticate_InvalidPassword_ReturnsNull()
        {
            _authService.Register("testuser", "password123");
            var user = _authService.Authenticate("testuser", "wrongpassword");

            Assert.IsNull(user);
        }

        [Test]
        public void ResetPassword_ValidUser_ReturnsTrue()
        {
            _authService.Register("testuser", "oldpass");
            var result = _authService.ResetPassword("testuser", "newpass");

            Assert.IsTrue(result);
            var user = _authService.Authenticate("testuser", "newpass");
            Assert.IsNotNull(user);
        }

        [Test]
        public void ResetPassword_NonExistentUser_ReturnsFalse()
        {
            var result = _authService.ResetPassword("nouser", "newpass");
            Assert.IsFalse(result);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
