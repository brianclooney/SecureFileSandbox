using Moq;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using RestFileService.Common.Services;
using System.Security.Claims;

namespace RestFileService.Tests.Common.Services;

public class AuthorizationServiceTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly AuthorizationService _authorizationService;
    private readonly DefaultHttpContext _httpContext;


    public AuthorizationServiceTests()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _httpContext = new DefaultHttpContext();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContext);
        _authorizationService = new AuthorizationService(_httpContextAccessorMock.Object);
    }

    [Fact]
    public void IsMemberOfGroup_ShouldReturnFalse_WhenGroupsClaimIsMissing()
    {
        // Arrange - No groups claim
        var claimsIdentity = new ClaimsIdentity();
        _httpContext.User = new ClaimsPrincipal(claimsIdentity);

        // Act
        var result = _authorizationService.IsMemberOfGroup("member", "read");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsMemberOfGroup_ShouldReturnFalse_WhenUserIsNotInAnyGroup()
    {
        // Arrange
        var claimsIdentity = new ClaimsIdentity(new Claim[]
        {
            new Claim("groups", "")
        });
        _httpContext.User = new ClaimsPrincipal(claimsIdentity);

        // Act
        var result = _authorizationService.IsMemberOfGroup("member", "read");

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("member", "read", "member:ro", true)]
    [InlineData("member", "read", "member:rw", true)]
    [InlineData("member", "write", "member:rw", true)]
    [InlineData("member", "write", "member:ro", false)]
    [InlineData("member", "read", "moderator:r0", false)]
    [InlineData("admin", "read", "member:rw", false)]
    public void IsMemberOfGroup_ShouldReturnExpected_WhenGivenGroupAndPermission(string requiredGroup, string permission, string actualGroup, bool expected)
    {
        // Arrange
        var claimsIdentity = new ClaimsIdentity(new Claim[]
        {
            new Claim("groups", "dummy"),
            new Claim("groups", actualGroup)
        });
        _httpContext.User = new ClaimsPrincipal(claimsIdentity);

        // Act
        var result = _authorizationService.IsMemberOfGroup(requiredGroup, permission);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void IsAdmin_ShouldReturnExpected_WhenGivenAdminClaim(string adminValue, bool expected)
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("admin", adminValue)
        };
        var claimsIdentity = new ClaimsIdentity(claims);
        _httpContext.User = new ClaimsPrincipal(claimsIdentity);

        // Act
        var result = _authorizationService.IsAdmin();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(new string[] { "admin" }, true)]
    [InlineData(new string[] { "user:rw", "admin" }, true)]
    [InlineData(new string[] { "user:rw", "moderator:ro" }, false)]
    [InlineData(new string[] { }, false)]
    public void IsAdmin_ShouldReturnExpected_WhenGivenGroup(string[] groups, bool expected)
    {
        // Arrange
        var claims = new List<Claim>();
        foreach (var group in groups)
        {
            claims.Add(new Claim("groups", group));
        }

        var claimsIdentity = new ClaimsIdentity(claims);
        _httpContext.User = new ClaimsPrincipal(claimsIdentity);

        // Act
        var result = _authorizationService.IsAdmin();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("resource:create", new string[] { "resource:create", "resource:delete" }, true)]
    [InlineData("resource:create", new string[] { "resource:create" }, true)]
    [InlineData("resource:read", new string[] { "resource:create", "resource:delete" }, false)]
    [InlineData("resource:delete", new string[] { "resource:create", "resource:delete", "resource:read" }, true)]
    [InlineData("resource:update", new string[] { }, false)]
    public void HasScopePermission_ShouldReturnExpected_WhenGivenScopes(string requiredScope, string[] existingScopes, bool expected)
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("scope", string.Join(" ", existingScopes))
        };
        var claimsIdentity = new ClaimsIdentity(claims);
        _httpContext.User = new ClaimsPrincipal(claimsIdentity);

        // Act
        var result = _authorizationService.HasScopePermission(requiredScope);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("12345678-1234-1234-1234-123456789abc", "12345678-1234-1234-1234-123456789abc", true)]
    [InlineData("12345678-1234-1234-1234-123456789abc", "abcd1234-abcd-abcd-abcd-abcdabcdabcd", false)]
    public void IsRequiredUser_ShouldReturnExpected_WhenGivenUserId(string currentUserGuid, string requiredUserGuid, bool expected)
    {
        // Arrange
        var claimsIdentity = new ClaimsIdentity(new Claim[]
        {
            new Claim("sub", currentUserGuid)
        });
        _httpContext.User = new ClaimsPrincipal(claimsIdentity);

        // Act
        var result = _authorizationService.IsRequiredUser(Guid.Parse(requiredUserGuid));

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void IsRequiredUser_ShouldReturnFalse_WhenUserIdClaimIsAbsent()
    {
        // Arrange
        var claimsIdentity = new ClaimsIdentity();  // No user ID claim provided
        _httpContext.User = new ClaimsPrincipal(claimsIdentity);

        // Act
        var result = _authorizationService.IsRequiredUser(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("files", "team", new string[] { "admin" }, true)]
    [InlineData("files", "team", new string[] { "team:rw" }, true)]
    [InlineData("files", "team", new string[] { "team:ro" }, false)]
    [InlineData("files", "team", new string[] { "files:rw" }, false)]
    [InlineData("files", "team", new string[] { }, false)]
    [InlineData("files", "docs", new string[] { "docs:ro", "admin" }, true)]
    public void CanPerformCreate_ShouldReturnTrue_WhenUserHasWritePermissionInGroup(string resourceType, string requiredGroup, string[] groupClaims, bool expected)
    {
        // Arrange
        var claims = new List<Claim>();
        foreach (var claim in groupClaims)
        {
            claims.Add(new Claim("groups", claim));
        }

        var claimsIdentity = new ClaimsIdentity(claims, "mock");
        _httpContext.User = new ClaimsPrincipal(claimsIdentity);

        // Act
        var result = _authorizationService.CanPerformCreate(resourceType, requiredGroup);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void CanPerformCreate_ShouldReturnTrue_WhenUserIsAdmin()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("admin", "true")
        };

        var claimsIdentity = new ClaimsIdentity(claims, "mock");
        _httpContext.User = new ClaimsPrincipal(claimsIdentity);

        // Act
        var result = _authorizationService.CanPerformCreate("files");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanPerformCreate_ShouldReturnTrue_WhenUserHasScopePermission()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("scope", "files:create")
        };

        var claimsIdentity = new ClaimsIdentity(claims, "mock");
        _httpContext.User = new ClaimsPrincipal(claimsIdentity);

        // Act
        var result = _authorizationService.CanPerformCreate("files");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanPerformCreate_ShouldReturnFalse_WhenNoRelevantClaimsPresent()
    {
        // Arrange
        var claimsIdentity = new ClaimsIdentity();
        _httpContext.User = new ClaimsPrincipal(claimsIdentity);

        // Act
        var result = _authorizationService.CanPerformCreate("files", "team");

        // Assert
        result.Should().BeFalse();
    }
}
