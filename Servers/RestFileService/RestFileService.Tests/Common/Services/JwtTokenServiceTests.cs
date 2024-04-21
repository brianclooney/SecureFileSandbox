using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using RestFileService.Common.Services;

namespace RestFileService.Tests.Common.Services;

public class JwtTokenServiceTests
{
    private JwtTokenService _jwtTokenService;

    public JwtTokenServiceTests()
    {
        _jwtTokenService = new JwtTokenService();
    }

    [Fact]
    public void GenerateTokenForUser_ShouldContainExpectedClaims()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fullName = "John Doe";
        var email = "john.doe@example.com";
        var groups = new List<string> { "Admin", "User" };

        // Act
        var token = _jwtTokenService.GenerateTokenForUser(userId, fullName, email, groups);

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

        // Act
        var token = _jwtTokenService.GenerateTokenForResource(resourceId);
        
        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Claims.FirstOrDefault(c => c.Type == "res").Should().NotBeNull();
        jwtToken.Claims.FirstOrDefault(c => c.Type == "res")?.Value.Should().Be(resourceId);
    }

    [Fact]
    public void ValidateToken_WithValidToken_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fullName = "Jane Doe";
        var email = "jane.doe@example.com";
        var groups = new List<string> { "User" };
        var token = _jwtTokenService.GenerateTokenForUser(userId, fullName, email, groups);

        // Act
        var result = _jwtTokenService.ValidateToken(token);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ReturnsFalse()
    {
        // Arrange
        var invalidToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJzb21lX3VzZXIifQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

        // Act
        var result = _jwtTokenService.ValidateToken(invalidToken);

        // Assert
        result.Should().BeFalse();
    }
}
