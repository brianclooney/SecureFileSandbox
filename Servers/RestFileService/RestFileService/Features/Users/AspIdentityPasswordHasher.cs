using Microsoft.AspNetCore.Identity;

namespace RestFileService.Features.Users;

public class AspIdentityPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly User _defaultUser = new User();

    public AspIdentityPasswordHasher()
    {
        _passwordHasher = new PasswordHasher<User>();
    }

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(_defaultUser, password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(_defaultUser, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success;
    }
}