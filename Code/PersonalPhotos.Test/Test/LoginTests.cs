namespace PersonalPhotos.Test.Test;

using Controllers;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Moq;

public class LoginTests
{
    private readonly LoginsController _controller;
    private readonly Mock<ILogins> _loginsMock;
    private readonly Mock<IHttpContextAccessor> _accesorMock;

    public LoginTests()
    {
        _loginsMock = new Mock<ILogins>();

        var session = Mock.Of<ISession>();
        var httpContext = Mock.Of<HttpContext>(x => x.Session == session);

        _accesorMock = new Mock<IHttpContextAccessor>();
        _accesorMock.Setup(x => x.HttpContext).Returns(httpContext);

        _controller = new LoginsController(_loginsMock.Object, _accesorMock.Object);
    }

    [Fact]
    public void Index_GivenNonReturnUrl_ReturnLoginView()
    {
        var result = _controller.Index() as ViewResult;

        Assert.NotNull(result);
        Assert.Equal("Login", result.ViewName, ignoreCase: true);
    }

    [Fact]
    public async Task Login_GivenInvalidModelState_ReturnLoginView()
    {
        _controller.ModelState.AddModelError("Test", "Test");
        var result = await _controller.Login(Mock.Of<LoginViewModel>()) as ViewResult;

        Assert.Equal("Login", result.ViewName, ignoreCase: true);
    }

    [Fact]
    public async Task Login_GivenCorrectPassword_RedirectsToDisplayAction()
    {
        const string password = "123";
        var modelViewMock = Mock.Of<LoginViewModel>(x => x.Email == "myemail@.email.com" && x.Password == password);
        var modelMock = Mock.Of<User>(x => x.Password == password);

        _loginsMock.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(modelMock);

        var result = await _controller.Login(modelViewMock);

        Assert.IsType<RedirectToActionResult>(result);
    }

    [Fact]
    public async Task Login_GivenIncorrectPassword_RedirectsToViewResult()
    {
        var modelViewMock = Mock.Of<LoginViewModel>(x => x.Email == "myemail@.email.com" && x.Password == "password");
        var modelMock = Mock.Of<User>(x => x.Password == "IncorrectPassword");

        _loginsMock.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(modelMock);

        var result = await _controller.Login(modelViewMock);

        Assert.IsType<ViewResult>(result);
    }
}
