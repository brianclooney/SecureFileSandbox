using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestFileService.Common.Services;

public class JwtTokenService : ITokenService
{
    private const string SecretKey = "your_very_long_secret_here_more_than_128_bits";
    private readonly SymmetricSecurityKey _signingKey;
    private readonly IDateTimeProvider _dateTimeProvider;

    public JwtTokenService(IDateTimeProvider dateTimeProvider)
    {
        _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        _dateTimeProvider = dateTimeProvider;
    }

    public string GenerateTokenForResource(string scope, TimeSpan? lifeSpan = null)
    {
        var claims = new List<Claim>
        {
            new Claim("scope", scope),
            new Claim("jti", Guid.NewGuid().ToString())
        };

        return GenerateToken(claims, lifeSpan ?? TimeSpan.FromMinutes(60));
    }


    public string GenerateTokenForResources(List<string> scopes, TimeSpan? lifeSpan = null)
    {
        var claims = new List<Claim>
        {
            new Claim("scope", string.Join(" ", scopes)),
            new Claim("jti", Guid.NewGuid().ToString())
        };

        return GenerateToken(claims, lifeSpan ?? TimeSpan.FromMinutes(60));
    }

    public string GenerateTokenForUser(Guid userId, string userName, string email, List<string> groups, TimeSpan? lifeSpan = null)
    {
        var claims = new List<Claim>
        {
            new Claim("name", userName),
            new Claim("email", email),
            new Claim("sub", userId.ToString()),
            new Claim("jti", Guid.NewGuid().ToString()),
            new Claim("admin", true.ToString().ToLower())
        };

        foreach (var group in groups)
        {
            claims.Add(new Claim("groups", group));
        }

        return GenerateToken(claims, lifeSpan ?? TimeSpan.FromMinutes(15));
    }

    public bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _signingKey,
            ValidateIssuer = true,
            ValidIssuer = "MyIssuer",
            ValidateAudience = true,
            ValidAudience = "MyAudience",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private string GenerateToken(List<Claim> claims, TimeSpan lifeSpan)
    {
        var token = new JwtSecurityToken(
            issuer: "MyIssuer",
            audience: "MyAudience",
            claims: claims,
            expires: _dateTimeProvider.UtcNow.Add(lifeSpan),
            signingCredentials: new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
