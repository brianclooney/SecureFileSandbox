using Moq;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http.HttpResults;
using RestFileService.Features.Users;
using RestFileService.Features.Users.Endpoints;

namespace RestFileService.Tests.Features.Users.Endpoints;

public class CreateUserTests
{
    private CreateUserRequestValidator validator;

    public CreateUserTests()
    {
        validator = new CreateUserRequestValidator();
    }

    [Fact]
    public async void CreateUser_ValidRequest_ReturnsCreatedResponse()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();
        var passwordHasherMock = new Mock<IPasswordHasher>();
        var userId = Guid.NewGuid();
        var request = new CreateUserRequest("John", "Doe", "johndoe", "john.doe@example.com", "password123");
        
        passwordHasherMock.Setup(ph => ph.HashPassword("password123")).Returns("hashedPassword");
        repositoryMock.Setup(repo => repo.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(userId);

        var createUserModule = new CreateUser();

        // Act
        var result = await createUserModule.CreateUserDelegate(request, repositoryMock.Object, passwordHasherMock.Object);

        // Assert
        var createdResult = Assert.IsType<Created<CreateUserResponse>>(result);
        createdResult.Location.Should().Be($"/users/{userId}");
        createdResult.Value.Should().NotBeNull();
        createdResult.Value?.Id.Should().Be(userId);

        repositoryMock.Verify(x => x.CreateUserAsync(It.IsAny<User>()), Times.Once);
        passwordHasherMock.Verify(x => x.HashPassword("password123"), Times.Once); 
    }

    [Fact]
    public void CreateUserRequestValidator_WhenRequestIsValid_ShouldNotReturnValidationErrors()
    {
        var request = new CreateUserRequest("John", "Doe", "johndoe", "john.doe@example.com", "password123");
        var result = validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateUserRequestValidator_WhenFirstNameIsEmpty_ShouldReturnValidationErrors()
    {
        var request = new CreateUserRequest("", "Doe", "johndoe", "john.doe@example.com", "password123");
        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(user => user.FirstName);
    }

    [Fact]
    public void CreateUserRequestValidator_WhenFirstNameIsTooLong_ShouldReturnValidationErrors()
    {
        var request = new CreateUserRequest(new string('A', 51), "Doe", "johndoe", "john.doe@example.com", "password123");
        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(user => user.FirstName);
    }

    [Fact]
    public void CreateUserRequestValidator_WhenLastNameIsEmpty_ShouldReturnValidationErrors()
    {
        var request = new CreateUserRequest("John", "", "johndoe", "john.doe@example.com", "password123");
        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(user => user.LastName);
    }

    [Fact]
    public void CreateUserRequestValidator_WhenLastNameIsTooLong_ShouldReturnValidationErrors()
    {
        var request = new CreateUserRequest("John", new string('A', 51), "johndoe", "john.doe@example.com", "password123");
        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(user => user.LastName);
    }

    [Fact]
    public void CreateUserRequestValidator_WhenEmailIsEmpty_ShouldReturnValidationErrors()
    {
        var request = new CreateUserRequest("John", "Doe", "johndoe", "", "password123");
        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(user => user.Email);
    }

    [Fact]
    public void CreateUserRequestValidator_WhenEmailIsInvalid_ShouldReturnValidationErrors()
    {
        var request = new CreateUserRequest("John", "Doe", "johndoe", "myemail", "password123");
        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(user => user.Email);
    }

    [Fact]
    public void CreateUserRequestValidator_WhenUserNameIsEmpty_ShouldReturnValidationErrors()
    {
        var request = new CreateUserRequest("John", "Doe", "", "john.doe@example.com", "password123");
        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(user => user.UserName);
    }

    [Fact]
    public void CreateUserRequestValidator_WhenUserNameIsTooLong_ShouldReturnValidationErrors()
    {
        var request = new CreateUserRequest("John", "Doe", new string('A', 51), "john.doe@example.com", "password123");
        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(user => user.UserName);
    }

    [Fact]
    public void CreateUserRequestValidator_WhenPasswordIsTooShort_ShouldReturnValidationErrors()
    {
        var request = new CreateUserRequest("John", "Doe", "jdoe", "john.doe@example.com", "12345");
        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(user => user.Password);
    }

    [Fact]
    public void CreateUserRequestValidator_WhenAllFieldsAreInvalid_ShouldReturnValidationErrors()
    {
        var request = new CreateUserRequest("", "", "", "", "");
        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(user => user.FirstName);
        result.ShouldHaveValidationErrorFor(user => user.LastName);
        result.ShouldHaveValidationErrorFor(user => user.Email);
        result.ShouldHaveValidationErrorFor(user => user.UserName);
        result.ShouldHaveValidationErrorFor(user => user.Password);
    }
}
