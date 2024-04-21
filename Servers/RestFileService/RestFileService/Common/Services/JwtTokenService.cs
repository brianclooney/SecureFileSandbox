using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestFileService.Common.Services;

public class JwtTokenService : ITokenService
{
    private const string SecretKey = "your_very_long_secret_here_more_than_128_bits";
    private readonly SymmetricSecurityKey _signingKey;

    public JwtTokenService()
    {
        _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
    }

    public string GenerateTokenForResource(string resourceId)
    {
        var claims = new List<Claim>
        {
            new Claim("res", resourceId),
            new Claim("jti", Guid.NewGuid().ToString())
        };

        return GenerateToken(claims);
    }

    public string GenerateTokenForUser(Guid userId, string userName, string email, List<string> groups)
    {
        var claims = new List<Claim>
        {
            new Claim("name", userName),
            new Claim("email", email),
            new Claim("sub", userId.ToString()),
            new Claim("jti", Guid.NewGuid().ToString())
        };

        foreach (var group in groups)
        {
            claims.Add(new Claim("groups", group));
        }

        return GenerateToken(claims);
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

    private string GenerateToken(List<Claim> claims)
    {
        var token = new JwtSecurityToken(
            issuer: "MyIssuer",
            audience: "MyAudience",
            claims: claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
