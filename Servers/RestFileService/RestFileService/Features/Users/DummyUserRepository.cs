namespace RestFileService.Features.Users;

public class DummyUserRepository : IUserRepository
{
    public async Task<Guid> CreateUserAsync(User user)
    {
        return await Task.FromResult(Guid.NewGuid());
    }

    public async Task<User?> GetUserByUserNameAsync(string username)
    {
        return await Task.FromResult(User.Create("John", "Doe", "johndoe", "jdoe@example.com", "PASSWORDHASH"));
    }
}