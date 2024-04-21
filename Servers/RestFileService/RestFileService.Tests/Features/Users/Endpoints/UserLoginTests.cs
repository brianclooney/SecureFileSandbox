using Moq;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http.HttpResults;
// using Microsoft.AspNetCore.Mvc;
using RestFileService.Features.Users;
using RestFileService.Features.Users.Endpoints;
using RestFileService.Common.Services;

namespace RestFileService.Tests.Features.Users.Endpoints;

public class UserLoginTests
{
    private UserLogin _userLoginModule;

    public UserLoginTests()
    {
        _userLoginModule = new UserLogin();
    }

    [Fact]
    public async void UserLogin_ShouldReturnOkResponse_WhenRequestIsvalid()
    {
        // Arrange
        var enteredPassword = "Password1234";
        var hashedPassword = "hashedPassword";
        var repositoryMock = new Mock<IUserRepository>();
        var passwordHasherMock = new Mock<IPasswordHasher>();
        var tokenServiceMock = new Mock<ITokenService>();
        var user = User.Create("John", "Doe", "johndoe", "john.doe@example.com", hashedPassword);
        var request = new UserLoginRequest("johndoe", enteredPassword);

        passwordHasherMock.Setup(ph => ph.VerifyPassword(hashedPassword, enteredPassword)).Returns(true);
        repositoryMock.Setup(repo => repo.GetUserByUserNameAsync("johndoe")).ReturnsAsync(user);
        // tokenServiceMock.Setup(ts => ts.GenerateTokenForResource("files", It.IsAny<string>())).Returns("valid_token_string");
        tokenServiceMock
            .Setup(ts => ts.GenerateTokenForUser(user.Id, user.FullName, user.Email, It.IsAny<List<string>>(), null))
            .Returns("valid_token_string");

        // Act
        var result = await _userLoginModule.UserLoginDelegate(request, repositoryMock.Object, passwordHasherMock.Object, tokenServiceMock.Object);

        // Assert
        var okResult = Assert.IsType<Ok<UserLoginResponse>>(result);
        okResult.Value.Should().NotBeNull();
        okResult.Value?.Token.Should().NotBeNull();
        okResult.Value?.Token?.Should().Be("valid_token_string");
        repositoryMock.Verify(x => x.GetUserByUserNameAsync("johndoe"), Times.Once);
        passwordHasherMock.Verify(x => x.VerifyPassword(hashedPassword, enteredPassword), Times.Once);
    }

    [Fact]
    public async void UserLogin_ShouldReturnUnauthorizedResponse_WhenUserNameIsInvalid()
    {
        // Arrange
        var enteredPassword = "Password1234";
        var hashedPassword = "hashedPassword";
        var repositoryMock = new Mock<IUserRepository>();
        var passwordHasherMock = new Mock<IPasswordHasher>();
        var tokenServiceMock = new Mock<ITokenService>();
        var request = new UserLoginRequest("johndoe", enteredPassword);

        repositoryMock.Setup(repo => repo.GetUserByUserNameAsync("johndoe")).ReturnsAsync((User?)null);

        // Act
        var result = await _userLoginModule.UserLoginDelegate(request, repositoryMock.Object, passwordHasherMock.Object, tokenServiceMock.Object);

        // Assert
        Assert.IsType<UnauthorizedHttpResult>(result);
        repositoryMock.Verify(x => x.GetUserByUserNameAsync("johndoe"), Times.Once);
        passwordHasherMock.Verify(x => x.VerifyPassword(hashedPassword, enteredPassword), Times.Never);
    }

    [Fact]
    public async void UserLogin_ShouldReturnUnauthorizedResponse_WhenPasswordIsInvalid()
    {
        // Arrange
        var enteredPassword = "Password1234";
        var hashedPassword = "hashedPassword";
        var repositoryMock = new Mock<IUserRepository>();
        var passwordHasherMock = new Mock<IPasswordHasher>();
        var tokenServiceMock = new Mock<ITokenService>();
        var user = User.Create("John", "Doe", "johndoe", "john.doe@example.com", hashedPassword);
        var request = new UserLoginRequest("johndoe", enteredPassword);

        passwordHasherMock.Setup(ph => ph.VerifyPassword(hashedPassword, enteredPassword)).Returns(false);
        repositoryMock.Setup(repo => repo.GetUserByUserNameAsync("johndoe")).ReturnsAsync(user);

        // Act
        var result = await _userLoginModule.UserLoginDelegate(request, repositoryMock.Object, passwordHasherMock.Object, tokenServiceMock.Object);

        // Assert
        Assert.IsType<UnauthorizedHttpResult>(result);
        repositoryMock.Verify(x => x.GetUserByUserNameAsync("johndoe"), Times.Once);
        passwordHasherMock.Verify(x => x.VerifyPassword(hashedPassword, enteredPassword), Times.Once);
    }

    [Fact]
    public void UserLoginRequestValidator_ShouldNotReturnValidationErrors_WhenRequestIsValid()
    {
        // Arrange
        var request = new UserLoginRequest("johndoe", "Password1234");

        // Act
        var result = _userLoginModule.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UserLoginRequestValidator_ShouldReturnValidationErrors_WhenUserNameIsEmpty()
    {
        // Arrange
        var request = new UserLoginRequest("", "Password1234");

        // Act
        var result = _userLoginModule.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(user => user.UserName);
    }

    [Fact]
    public void UserLoginRequestValidator_ShouldReturnValidationErrors_WhenPasswordIsEmpty()
    {
        // Arrange
        var request = new UserLoginRequest("johndoe", "");

        // Act
        var result = _userLoginModule.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(user => user.Password);
    }
}
