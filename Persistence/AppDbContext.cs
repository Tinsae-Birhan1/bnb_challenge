using Microsoft.EntityFrameworkCore;
using Domain.Entities;


namespace Persistence.DbContexts;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Domain.Entities.User> Users { get; set; }
    public DbSet<Domain.Entities.TokenSupply> TokenSupplies { get; set; }
}
