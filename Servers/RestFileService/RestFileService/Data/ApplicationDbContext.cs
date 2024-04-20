using Microsoft.EntityFrameworkCore;
using RestFileService.Features.Users;

namespace RestFileService.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}
