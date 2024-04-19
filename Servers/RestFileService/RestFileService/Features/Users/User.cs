namespace RestFileService.Features.Users;

public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public bool IsActive { get; set; }
    public DateTime JoinedOn { get; set; } = DateTime.UtcNow;

    public string FullName => $"{FirstName} {LastName}";

    public static User Create(string firstName, string lastName, string userName, string email, string passwordHash)
    {
        return new User
        {
            FirstName = firstName,
            LastName = lastName,
            UserName = userName,
            Email = email,
            PasswordHash = passwordHash,
            IsActive = true,
            JoinedOn = DateTime.UtcNow
        };
    }
}
