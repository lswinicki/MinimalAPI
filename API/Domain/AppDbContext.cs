using Microsoft.EntityFrameworkCore;

namespace API.Domain;

public class AppDbContext : DbContext
{
    public DbSet<Entities.Team> Teams { get; set; }
    public DbSet<Entities.Player> Players { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}