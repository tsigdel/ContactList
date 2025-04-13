using ContactList.Web.Controllers;
using ContactList.Web.Models;
using ContactList.Web.Services;
using ContactList.Web.Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ContactList.Tests.Controllers
{
    [TestFixture]
    public class AccountControllerTests : IDisposable
    {
        private Mock<IAuthService> _mockAuthService;
        private Mock<IRedisService> _mockRedisService;
        private Mock<ILogger<AccountController>> _mockLogger;
        private AccountController _controller;
        private Mock<HttpContext> _mockHttpContext;

        [SetUp]
        public void Setup()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockRedisService = new Mock<IRedisService>();
            _mockLogger = new Mock<ILogger<AccountController>>();
            _mockHttpContext = new Mock<HttpContext>();
            _controller = new AccountController(
                _mockAuthService.Object,
                _mockRedisService.Object,
                _mockLogger.Object
            );
        }

        // Implement IDisposable for cleanup
        [TearDown]
        public void Dispose()
        {
            // Clean up mock dependencies
            _mockAuthService = null;
            _mockRedisService = null;
            _mockLogger = null;

            // Null out references to controller (not necessary to dispose)
            _controller = null;
        }

        [Test]
        public async Task Login_InvalidCredentials_ReturnsViewWithError()
        {
            // Arrange
            var username = "wrong";
            var password = "wrongpass";
            _mockAuthService.Setup(x => x.Authenticate(username, password)).Returns((User)null);

            // Act
            var result = await _controller.Login(username, password);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var view = (ViewResult)result;
            Assert.AreEqual("Invalid credentials.", view.ViewData["Error"]);
        }


        [Test]
        public void Reset_Get_ReturnsView()
        {
            // Act
            var result = _controller.Reset();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Reset_InvalidInput_ReturnsViewWithError()
        {
            // Act
            var result = _controller.Reset("", "", "") as ViewResult;

            // Assert
            Assert.AreEqual("Username and both password fields are required.", result.ViewData["Error"]);
        }

        [Test]
        public void Reset_PasswordsDoNotMatch_ReturnsViewWithError()
        {
            // Act
            var result = _controller.Reset("user", "Password1", "Password2") as ViewResult;

            // Assert
            Assert.AreEqual("Passwords do not match.", result.ViewData["Error"]);
        }

        [Test]
        public void Reset_WeakPassword_ReturnsViewWithError()
        {
            // Act
            var result = _controller.Reset("user", "short", "short") as ViewResult;

            // Assert
            Assert.AreEqual("Password must be at least 8 characters long and contain both letters and numbers.", result.ViewData["Error"]);
        }

        [Test]
        public void Reset_UserNotFound_ReturnsViewWithError()
        {
            _mockAuthService.Setup(x => x.ResetPassword("user", "Password123")).Returns(false);

            var result = _controller.Reset("user", "Password123", "Password123") as ViewResult;

            Assert.AreEqual("User not found or password reset failed.", result.ViewData["Error"]);
        }

        [Test]
        public void Reset_Valid_ReturnsRedirectToLogin()
        {
            _mockAuthService.Setup(x => x.ResetPassword("user", "Password123")).Returns(true);

            var result = _controller.Reset("user", "Password123", "Password123");

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual("Login", ((RedirectToActionResult)result).ActionName);
        }

    }
}
