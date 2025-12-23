using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Falcon.Infrastructure.Database;

/// <summary>
/// FalconDbContextFactory is used by EF Core tools at design time to create
/// instances of FalconDbContext for things like migrations.
/// </summary>
public class FalconDbContextFactory : IDesignTimeDbContextFactory<FalconDbContext>
{
    public FalconDbContext CreateDbContext(string[] args)
    {
        // 1. Configures where to read the connection string
        // We need to point to the API folder to read the appsettings.json from there
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../Falcon.Api");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        // 2. Configure the Database Builder
        var builder = new DbContextOptionsBuilder<FalconDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        builder.UseSqlServer(connectionString, b => b.MigrationsAssembly("Falcon.Infrastructure"));

        // 3. Return the Context
        return new FalconDbContext(builder.Options);
    }
}
