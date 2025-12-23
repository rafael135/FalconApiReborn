using Falcon.Core;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Infrastructure.Database;

public class FalconDbContext : DbContext
{
    public FalconDbContext(DbContextOptions<FalconDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all configurations from the current assembly
        // This assumes that entity configurations are defined in separate classes
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FalconDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
