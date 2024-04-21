namespace RestFileService.Common.Services;


public interface ITokenService
{
    /// <summary>
    /// Generates a token for a user with specific attributes and group memberships, with an optional expiry.
    /// </summary>
    /// <param name="userId">The unique identifier for the user.</param>
    /// <param name="userName">The user's name.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="groups">A list of group names to which the user belongs.</param>
    /// <param name="expiry">The duration in minutes for which the token should remain valid. Default is 15 minutes.</param>
    /// <returns>A string representing the user's authentication token.</returns>
    string GenerateTokenForUser(Guid userId, string userName, string email, List<string> groups, TimeSpan? expiry = null);

    /// <summary>
    /// Generates a token with a specific access scope for a resource, with an optional expiry.
    /// </summary>
    /// <param name="scope">The scope string defining access parameters for the resource.</param>
    /// <param name="expiry">The duration in minutes for which the token should remain valid. Default is 60 minutes.</param>
    /// <returns>A string representing the resource access token.</returns>
    string GenerateTokenForResource(string scope, TimeSpan? expiry = null);

    /// <summary>
    /// Generates a token that includes multiple scopes for accessing various resources, with an optional expiry.
    /// </summary>
    /// <param name="scopes">A list of scope strings, each defining access parameters for different resources.</param>
    /// <param name="expiry">The duration in minutes for which the token should remain valid. Default is 60 minutes.</param>
    /// <returns>A string representing the access token that encompasses multiple resources.</returns>
    string GenerateTokenForResources(List<string> scopes, TimeSpan? expiry = null);

    /// <summary>
    /// Validates a given token to check its integrity and applicability.
    /// </summary>
    /// <param name="token">The token string to validate.</param>
    /// <returns>True if the token is valid, otherwise false.</returns>
    bool ValidateToken(string token);
}
