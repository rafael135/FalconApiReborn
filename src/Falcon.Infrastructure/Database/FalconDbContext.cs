using Falcon.Core.Domain.Groups;
using Falcon.Core.Domain.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Infrastructure.Database;

public class FalconDbContext : IdentityDbContext<User>
{
    public FalconDbContext(DbContextOptions<FalconDbContext> options)
        : base(options) { }

    public DbSet<Group> Groups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        // This assumes that entity configurations are defined in separate classes
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FalconDbContext).Assembly);
    }
}
