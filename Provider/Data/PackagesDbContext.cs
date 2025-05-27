using Microsoft.EntityFrameworkCore;
using Provider.Models;

namespace Provider.Data;

public class PackagesDbContext(DbContextOptions<PackagesDbContext> options) : DbContext(options)
{
    public DbSet<Package> Packages { get; set; } = null!;
    public DbSet<Benefit> Benefits { get; set; } = null!;
}
