using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using RestFileService.Common.Services;
using Moq;

namespace RestFileService.Tests.Common.Services;

public class JwtTokenServiceTests
{
    // private JwtTokenService _jwtTokenService;

    public JwtTokenServiceTests()
    {
        // _jwtTokenService = new JwtTokenService();
    }

    [Fact]
    public void GenerateTokenForUser_ShouldContainExpectedClaims()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fullName = "John Doe";
        var email = "john.doe@example.com";
        var groups = new List<string> { "Admin", "User" };

        var mockDataTimeProvider = new Mock<IDateTimeProvider>();

        var now = DateTime.UtcNow;
        mockDataTimeProvider.Setup(x => x.UtcNow).Returns(now);

        var jwtTokenService = new JwtTokenService(mockDataTimeProvider.Object);

        // Act
        var token = jwtTokenService.GenerateTokenForUser(userId, fullName, email, groups);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Claims.FirstOrDefault(c => c.Type == "sub").Should().NotBeNull();
        jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value.Should().Be(userId.ToString());

        jwtToken.Claims.FirstOrDefault(c => c.Type == "name").Should().NotBeNull();
        jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value.Should().Be(fullName);

        jwtToken.Claims.FirstOrDefault(c => c.Type == "email").Should().NotBeNull();
        jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value.Should().Be(email);

        jwtToken.Claims.Where(c => c.Type == "groups").Should().NotBeNull();
        jwtToken.Claims.Where(c => c.Type == "groups")?.Select(c => c.Value).Should().Contain(groups);
    }

    [Fact]
    public void GenerateTokenForResource_ShouldIncludeResourceId()
    {
        // Arrange
        var resourceId = "12345";

        var mockDataTimeProvider = new Mock<IDateTimeProvider>();

        var now = DateTime.UtcNow;
        mockDataTimeProvider.Setup(x => x.UtcNow).Returns(now);

        var jwtTokenService = new JwtTokenService(mockDataTimeProvider.Object);

        // Act
        var token = jwtTokenService.GenerateTokenForResource($"files:read:{resourceId}");
        
        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Claims.FirstOrDefault(c => c.Type == "scope").Should().NotBeNull();
        jwtToken.Claims.FirstOrDefault(c => c.Type == "scope")?.Value.Should().Be($"files:read:{resourceId}");
    }

    [Fact]
    public void ValidateToken_ShouldReturnTrue_WhenTokenIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fullName = "Jane Doe";
        var email = "jane.doe@example.com";
        var groups = new List<string> { "User" };

        var mockDataTimeProvider = new Mock<IDateTimeProvider>();

        var now = DateTime.UtcNow;
        mockDataTimeProvider.Setup(x => x.UtcNow).Returns(now);

        var jwtTokenService = new JwtTokenService(mockDataTimeProvider.Object);

        var token = jwtTokenService.GenerateTokenForUser(userId, fullName, email, groups);

        // Act
        var result = jwtTokenService.ValidateToken(token);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateToken_ShouldReturnFalse_WhenTokenHasExpired()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fullName = "Jane Doe";
        var email = "jane.doe@example.com";
        var groups = new List<string> { "User" };

        var mockDataTimeProvider = new Mock<IDateTimeProvider>();

        var now = DateTime.UtcNow.AddMinutes(-30);
        mockDataTimeProvider.Setup(x => x.UtcNow).Returns(now);

        var jwtTokenService = new JwtTokenService(mockDataTimeProvider.Object);

        var token = jwtTokenService.GenerateTokenForUser(userId, fullName, email, groups);

        // Act
        var result = jwtTokenService.ValidateToken(token);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateToken_ShouldReturnFalse_WhenTokenIsInvalid()
    {
        // Arrange
        var invalidToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJzb21lX3VzZXIifQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

        var mockDataTimeProvider = new Mock<IDateTimeProvider>();

        var now = DateTime.UtcNow;
        mockDataTimeProvider.Setup(x => x.UtcNow).Returns(now);

        var jwtTokenService = new JwtTokenService(mockDataTimeProvider.Object);

        // Act
        var result = jwtTokenService.ValidateToken(invalidToken);

        // Assert
        result.Should().BeFalse();
    }

    
}
