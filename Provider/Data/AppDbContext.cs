using Microsoft.EntityFrameworkCore;
using Provider.Models;

namespace Provider.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Package> Packages { get; set; } = null!;
    public DbSet<Benefit> Benefits { get; set; } = null!;
}
