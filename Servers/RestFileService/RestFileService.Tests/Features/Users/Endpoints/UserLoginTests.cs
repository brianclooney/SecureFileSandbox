using Moq;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http.HttpResults;
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

        passwordHasherMock.Setup(ph => ph.HashPassword(enteredPassword)).Returns(hashedPassword);
        repositoryMock.Setup(repo => repo.GetUserByLoginAsync("johndoe", hashedPassword)).ReturnsAsync(user);

        var userLoginModule = new UserLogin();

        // Act
        var result = await userLoginModule.UserLoginDelegate(request, repositoryMock.Object, passwordHasherMock.Object);

        // Assert
        var okResult = Assert.IsType<Ok<UserLoginResponse>>(result);
        okResult.Value.Should().NotBeNull();
        okResult.Value?.IsSuccess.Should().Be(true);
        repositoryMock.Verify(x => x.GetUserByLoginAsync("johndoe", hashedPassword), Times.Once);
        passwordHasherMock.Verify(x => x.HashPassword(enteredPassword), Times.Once);
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
        result.ShouldHaveValidationErrorFor(user => user.UserNameOrEmail);
    }

    [Fact]
    public void UserLoginRequestValidator_WhenPasswordIsEmpty_ShouldReturnValidationErrors()
    {
        var request = new UserLoginRequest("johndoe", "");
        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(user => user.Password);
    }
}
