namespace RestFileService.Features.Users;

public interface IUserRepository
{
    Task<Guid> CreateUserAsync(User user);
    Task DeleteUserAsync(Guid id);
    Task<List<User>> GetAllUsersAsync();
    Task UpdateUserAsync(User user);
    Task<User> GetUserByIdAsync(Guid id);
    Task<User> GetUserByNameAsync(string name);
    Task<User> GetUserByLoginAsync(string userNameOrEmail, string password);
}
