using ContactList.Web.Controllers;
using ContactList.Web.Models;  // Make sure you include the appropriate namespace for the User class
using ContactList.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using System.Threading.Tasks;
using ContactList.Web.Common.Services;

namespace ContactList.Tests.Controllers
{
    [TestFixture]
    public class AccountControllerTests : IDisposable
    {
        private Mock<IRedisService> _mockRedisService;
        private Mock<IAuthService> _mockAuthService;
        private Mock<ILogger<AccountController>> _mockLogger;
        private AccountController _controller;

        public AccountControllerTests()
        {
            // Constructor can be used for any setup logic if necessary
        }

        [SetUp]
        public void SetUp()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockLogger = new Mock<ILogger<AccountController>>();
            _controller = new AccountController(_mockAuthService.Object, _mockRedisService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task Login_InvalidCredentials_ReturnsViewWithError()
        {
            // Arrange
            var username = "invaliduser";
            var password = "invalidpassword";

            // Return null for invalid credentials
            _mockAuthService.Setup(service => service.Authenticate(username, password))
                .Returns((User)null);  // Return null to simulate invalid credentials

            // Act
            var result = await _controller.Login(username, password);

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.AreEqual("Invalid credentials.", viewResult.ViewData["Error"]);
        }

        [Test]
        public void Register_ValidRegistration_RedirectsToLogin()
        {
            // Arrange
            var username = "newuser";
            var password = "newpassword123";

            _mockAuthService.Setup(service => service.Register(username, password))
                .Returns(true);

            // Act
            var result = _controller.Register(username, password);  // No await, as it's not async

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.NotNull(redirectResult);
            Assert.AreEqual("Login", redirectResult.ActionName);
        }


        [Test]
        public void Register_InvalidRegistration_ReturnsViewWithError()
        {
            // Arrange
            var username = "";
            var password = "";

            // Act
            var result = _controller.Register(username, password);  // No await, as it's not async

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.AreEqual("Username and password are required.", viewResult.ViewData["Error"]);
        }


        // Implement IDisposable for cleanup
        [TearDown]
        public void Dispose()
        {
            // You can clean up here if necessary
            _mockAuthService = null;  // Null out references if required
            _controller = null;  // Null out references if needed
        }
    }
}
