using Moq;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http.HttpResults;
// using Microsoft.AspNetCore.Mvc;
using RestFileService.Features.Users;
using RestFileService.Features.Users.Endpoints;

namespace RestFileService.Tests.Features.Users.Endpoints;

public class UserLoginTests
{
    private UserLoginRequestValidator validator;

    public UserLoginTests()
    {
        validator = new UserLoginRequestValidator();
    }

    [Fact]
    public async void UserLogin_ValidRequest_ReturnsOkResponse()
    {
        // Arrange
        var enteredPassword = "Password1234";
        var hashedPassword = "hashedPassword";
        var repositoryMock = new Mock<IUserRepository>();
        var passwordHasherMock = new Mock<IPasswordHasher>();
        var user = User.Create("John", "Doe", "johndoe", "john.doe@example.com", hashedPassword);
        var request = new UserLoginRequest("johndoe", enteredPassword);

        passwordHasherMock.Setup(ph => ph.VerifyPassword(hashedPassword, enteredPassword)).Returns(true);
        repositoryMock.Setup(repo => repo.GetUserByUserNameAsync("johndoe")).ReturnsAsync(user);

        var userLoginModule = new UserLogin();

        // Act
        var result = await userLoginModule.UserLoginDelegate(request, repositoryMock.Object, passwordHasherMock.Object);

        // Assert
        var okResult = Assert.IsType<Ok<UserLoginResponse>>(result);
        okResult.Value.Should().NotBeNull();
        okResult.Value?.IsSuccess.Should().Be(true);
        repositoryMock.Verify(x => x.GetUserByUserNameAsync("johndoe"), Times.Once);
        passwordHasherMock.Verify(x => x.VerifyPassword(hashedPassword, enteredPassword), Times.Once);
    }

    [Fact]
    public async void UserLogin_InvalidUserName_ReturnsUnauthorizedResponse()
    {
        // Arrange
        var enteredPassword = "Password1234";
        var hashedPassword = "hashedPassword";
        var repositoryMock = new Mock<IUserRepository>();
        var passwordHasherMock = new Mock<IPasswordHasher>();
        var request = new UserLoginRequest("johndoe", enteredPassword);

        repositoryMock.Setup(repo => repo.GetUserByUserNameAsync("johndoe")).ReturnsAsync((User?)null);

        var userLoginModule = new UserLogin();

        // Act
        var result = await userLoginModule.UserLoginDelegate(request, repositoryMock.Object, passwordHasherMock.Object);

        // Assert
        Assert.IsType<UnauthorizedHttpResult>(result);
        repositoryMock.Verify(x => x.GetUserByUserNameAsync("johndoe"), Times.Once);
        passwordHasherMock.Verify(x => x.VerifyPassword(hashedPassword, enteredPassword), Times.Never);
    }

    [Fact]
    public async void UserLogin_InvalidPassword_ReturnsUnauthorizedResponse()
    {
        // Arrange
        var enteredPassword = "Password1234";
        var hashedPassword = "hashedPassword";
        var repositoryMock = new Mock<IUserRepository>();
        var passwordHasherMock = new Mock<IPasswordHasher>();
        var user = User.Create("John", "Doe", "johndoe", "john.doe@example.com", hashedPassword);
        var request = new UserLoginRequest("johndoe", enteredPassword);

        passwordHasherMock.Setup(ph => ph.VerifyPassword(hashedPassword, enteredPassword)).Returns(false);
        repositoryMock.Setup(repo => repo.GetUserByUserNameAsync("johndoe")).ReturnsAsync(user);

        var userLoginModule = new UserLogin();

        // Act
        var result = await userLoginModule.UserLoginDelegate(request, repositoryMock.Object, passwordHasherMock.Object);

        // Assert
        Assert.IsType<UnauthorizedHttpResult>(result);
        repositoryMock.Verify(x => x.GetUserByUserNameAsync("johndoe"), Times.Once);
        passwordHasherMock.Verify(x => x.VerifyPassword(hashedPassword, enteredPassword), Times.Once);
    }

    [Fact]
    public void UserLoginRequestValidator_WhenRequestIsValid_ShouldNotReturnValidationErrors()
    {
        var request = new UserLoginRequest("johndoe", "Password1234");
        var result = validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UserLoginRequestValidator_WhenUserNameIsEmpty_ShouldReturnValidationErrors()
    {
        var request = new UserLoginRequest("", "Password1234");
        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(user => user.UserName);
    }

    [Fact]
    public void UserLoginRequestValidator_WhenPasswordIsEmpty_ShouldReturnValidationErrors()
    {
        var request = new UserLoginRequest("johndoe", "");
        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(user => user.Password);
    }
}
