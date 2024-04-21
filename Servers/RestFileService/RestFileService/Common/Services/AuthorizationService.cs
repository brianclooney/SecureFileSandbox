using System.Security.Claims;

namespace RestFileService.Common.Services;

public class AuthorizationService : IAuthorizationService
{
    private IHttpContextAccessor _httpContextAccessor;

    public AuthorizationService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool CanPerformCreate(string resourceType, string? requiredGroup = null)
    {
        return IsAdmin() || IsMemberOfGroup(requiredGroup, "write") || HasScopePermission($"{resourceType}:create");
    }

    public bool CanPerformUpdateOrDelete(Guid requiredUserId)
    {
        return IsAdmin() || IsRequiredUser(requiredUserId);
    }

    public bool CanPerformRead(string resourceType)
    {
        return IsAdmin() || HasScopePermission($"{resourceType}:read");
    }

    public bool CanPerformRead(string resourceType, string requiredGroup)
    {
        return IsAdmin() || IsMemberOfGroup(requiredGroup, "read");
    }

    public bool CanPerformRead(string resourceType, string requiredGroup, Guid resourceId)
    {
        return IsAdmin() || (HasScopePermission($"{resourceType}:read:{resourceId}") && IsMemberOfGroup(requiredGroup, "read"));
    }

    public bool CanPerformRead(string resourceType, Guid resourceId)
    {
        return IsAdmin() || HasScopePermission($"{resourceType}:read:{resourceId}");
    }

    public bool IsAdmin()
    {
        var userClaims = GetUserClaims();
        var groups = userClaims.Where(c => c.Type == "groups").Select(c => c.Value).ToList();
        return (groups != null && groups.Contains("admin")) || (userClaims.FirstOrDefault(c => c.Type == "admin")?.Value == "true");
    }

    public bool HasScopePermission(string requiredScope)
    {
        var scopes = GetUserClaims().FirstOrDefault(c => c.Type == "scope")?.Value?.ToLower().Split(' ');
        return scopes != null && scopes.Contains(requiredScope);
    }

    public bool IsMemberOfGroup(string? requiredGroup, string action)
    {
        if (string.IsNullOrEmpty(requiredGroup)) return false;

        var groups = GetUserClaims().Where(c => c.Type == "groups").Select(c => c.Value).ToList();
        if (groups == null) return false;

        var suffix = action == "read" ? ":r" : ":rw";
        return groups.Any(g => g.StartsWith($"{requiredGroup}{suffix}"));
    }

    public bool IsRequiredUser(Guid requiredUserId)
    {
        var subjectClaim = GetUserClaims().FirstOrDefault(c => c.Type == "sub")?.Value;
        if (Guid.TryParse(subjectClaim, out Guid userId))
        {
            return userId == requiredUserId;
        }
        return false;
    }

    private IEnumerable<Claim> GetUserClaims()
    {
        var claims = _httpContextAccessor.HttpContext?.User?.Claims;
        return claims != null ? claims : Enumerable.Empty<Claim>();
    }
}
