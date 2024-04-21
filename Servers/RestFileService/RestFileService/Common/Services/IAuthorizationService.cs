namespace RestFileService.Common.Services;

public interface IAuthorizationService
{
    bool CanPerformCreate(string resourceType, string? requiredGroup = null);
    bool CanPerformUpdateOrDelete(Guid requiredUserId);
    bool CanPerformRead(string resourceType);
    bool CanPerformRead(string resourceType, string requiredGroup);
    bool CanPerformRead(string resourceType, string requiredGroup, Guid resourceId);
    bool CanPerformRead(string resourceType, Guid resourceId);
    bool IsAdmin();
    bool HasScopePermission(string requiredScope);
    bool IsMemberOfGroup(string requiredGroup, string action);
    bool IsRequiredUser(Guid requiredUserId);
}
