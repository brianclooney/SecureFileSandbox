using Microsoft.EntityFrameworkCore;
using RestFileService.Data;

namespace RestFileService.Features.Users;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> CreateUserAsync(User user)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return user.Id;
    }

    public async Task<User?> GetUserByUserNameAsync(string username)
    {
        return await _dbContext.Users.Where(u => u.UserName == username).FirstOrDefaultAsync();
    }
}
