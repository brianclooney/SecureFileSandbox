namespace RestFileService.Features.Users;

public interface IUserRepository
{
    Task<Guid> CreateUserAsync(User user);
    Task<User?> GetUserByUserNameAsync(string username);

    // Task DeleteUserAsync(Guid id);
    // Task<List<User>> GetAllUsersAsync();
    // Task UpdateUserAsync(User user);
    // Task<User> GetUserByIdAsync(Guid id);
}
