namespace RestFileService.Common.Services;

public interface ITokenService
{
    string GenerateTokenForUser(Guid userId, string userName, string email, List<string> groups);
    string GenerateTokenForResource(string resourceId);
    bool ValidateToken(string token);
}
