using System.Text;
using Falcon.Core.Interfaces;
using Falcon.Infrastructure.Database;
using Falcon.Infrastructure.Storage;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace Falcon.Api.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public const string TestJwtSecretKey = "TestSecretKeyForIntegrationTests_Minimum32Characters!";
    public const string TestJwtIssuer = "TestIssuer";
    public const string TestJwtAudience = "TestAudience";

    // Use a unique database name per factory instance to ensure isolation
    // Each test class gets its own factory instance (IClassFixture), so tests within a class share the same DB
    // but different test classes are isolated
    private readonly string _databaseName = "TestDb_" + Guid.NewGuid().ToString("N")[..8];

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set environment to Testing to prevent SQL Server registration in DependencyInjection
        builder.UseEnvironment("Testing");

        // Configure app settings to remove SQL Server connection string
        builder.ConfigureAppConfiguration(
            (context, config) =>
            {
                config.AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        ["ConnectionStrings:DefaultConnection"] = "", // Empty to prevent SQL Server registration
                        ["Environment"] = "Testing",
                    }!
                );
            }
        );

        builder.ConfigureTestServices(services =>
        {
            // Remove all DbContext registrations (this runs AFTER the app's ConfigureServices)
            // Remove by service type
            var dbContextOptionsDescriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<FalconDbContext>))
                .ToList();
            foreach (var descriptor in dbContextOptionsDescriptors)
            {
                services.Remove(descriptor);
            }

            var dbContextDescriptors = services
                .Where(d => d.ServiceType == typeof(FalconDbContext))
                .ToList();
            foreach (var descriptor in dbContextDescriptors)
            {
                services.Remove(descriptor);
            }

            // Remove SQL Server specific services
            var sqlServerDescriptors = services
                .Where(d =>
                    d.ServiceType != null
                    && (
                        d.ServiceType.FullName?.Contains("SqlServer") == true
                        || d.ImplementationType?.FullName?.Contains("SqlServer") == true
                        || d.ImplementationFactory?.Method?.ReturnType?.FullName?.Contains(
                            "SqlServer"
                        ) == true
                    )
                )
                .ToList();
            foreach (var descriptor in sqlServerDescriptors)
            {
                services.Remove(descriptor);
            }

            // Add in-memory database - use instance-specific name for isolation between test classes
            // Tests within the same class will share the database (via IClassFixture)
            services.AddDbContext<FalconDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
                options.EnableSensitiveDataLogging();
            });

            // Remove MassTransit/RabbitMQ services and hosted services
            var massTransitDescriptors = services
                .Where(d =>
                    d.ServiceType.Name.Contains("MassTransit")
                    || d.ServiceType.Name.Contains("IBus")
                    || d.ServiceType.Name.Contains("IPublishEndpoint")
                    || d.ServiceType.Name.Contains("ISendEndpointProvider")
                    || d.ServiceType.Name.Contains("IBusDepot")
                    || d.ServiceType.Name.Contains("IBusControl")
                    || (
                        d.ImplementationType != null
                        && d.ImplementationType.Name.Contains("MassTransit")
                    )
                )
                .ToList();
            foreach (var massTransitDescriptor in massTransitDescriptors)
            {
                services.Remove(massTransitDescriptor);
            }

            // Remove MassTransit hosted services
            var hostedServices = services
                .Where(d =>
                    d.ServiceType == typeof(Microsoft.Extensions.Hosting.IHostedService)
                    && d.ImplementationType != null
                    && d.ImplementationType.Name.Contains("MassTransit")
                )
                .ToList();
            foreach (var hostedService in hostedServices)
            {
                services.Remove(hostedService);
            }

            // Mock MassTransit services
            var mockPublishEndpoint = new Mock<IPublishEndpoint>();
            services.AddSingleton(mockPublishEndpoint.Object);
            services.AddSingleton(mockPublishEndpoint);

            // Mock Judge Service
            var judgeServiceDescriptor = services.FirstOrDefault(s =>
                s.ServiceType == typeof(IJudgeService)
            );
            if (judgeServiceDescriptor != null)
            {
                services.Remove(judgeServiceDescriptor);
            }
            var mockJudgeService = new Mock<IJudgeService>();
            // Setup CreateExerciseAsync to return a valid UUID
            mockJudgeService
                .Setup(x =>
                    x.CreateExerciseAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<List<Falcon.Core.Interfaces.TestCase>>()
                    )
                )
                .ReturnsAsync(Guid.NewGuid().ToString());
            // Setup other methods as needed
            mockJudgeService
                .Setup(x =>
                    x.SubmitCodeAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())
                )
                .ReturnsAsync(
                    new Falcon.Core.Interfaces.JudgeSubmissionResult(
                        Guid.NewGuid().ToString(),
                        Falcon.Core.Domain.Shared.Enums.JudgeSubmissionResponse.Accepted,
                        TimeSpan.FromSeconds(1),
                        1024
                    )
                );
            services.AddScoped(_ => mockJudgeService.Object);
            services.AddSingleton(mockJudgeService);

            // Replace FileStorageService with temporary directory
            var fileStorageDescriptor = services.FirstOrDefault(d =>
                d.ServiceType == typeof(IFileStorageService)
            );
            if (fileStorageDescriptor != null)
            {
                services.Remove(fileStorageDescriptor);
            }
            var tempDir = Path.Combine(
                Path.GetTempPath(),
                "FalconApiTests_" + Guid.NewGuid().ToString()
            );
            Directory.CreateDirectory(tempDir);
            services.AddSingleton<IFileStorageService>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<LocalFileStorageService>>();
                return new TestFileStorageService(tempDir, logger);
            });

            // Configure JWT Authentication for tests
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(TestJwtSecretKey)
                        ),
                        ValidateIssuer = true,
                        ValidIssuer = TestJwtIssuer,
                        ValidateAudience = true,
                        ValidAudience = TestJwtAudience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                    };
                });
        });

        builder.ConfigureAppConfiguration(
            (context, config) =>
            {
                config.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        { "Jwt:SecretKey", TestJwtSecretKey },
                        { "Jwt:Issuer", TestJwtIssuer },
                        { "Jwt:Audience", TestJwtAudience },
                        { "ConnectionStrings:DefaultConnection", "" }, // Empty connection string to prevent SQL Server registration
                        { "ASPNETCORE_ENVIRONMENT", "Testing" }, // Set environment to Testing
                    }
                );
            }
        );

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
        });

        builder.UseEnvironment("Testing");
    }
}

// Test implementation of IFileStorageService that doesn't require IWebHostEnvironment
internal class TestFileStorageService : IFileStorageService
{
    private readonly string _baseDirectory;
    private readonly ILogger _logger;

    public TestFileStorageService(string baseDirectory, ILogger logger)
    {
        _baseDirectory = baseDirectory;
        _logger = logger;
        Directory.CreateDirectory(_baseDirectory);
    }

    public async Task<string> SaveFileAsync(
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default
    )
    {
        var extension = Path.GetExtension(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var now = DateTime.UtcNow;
        var yearMonth = Path.Combine(now.Year.ToString(), now.Month.ToString("D2"));
        var fullDirectory = Path.Combine(_baseDirectory, yearMonth);

        if (!Directory.Exists(fullDirectory))
        {
            Directory.CreateDirectory(fullDirectory);
        }

        var fullPath = Path.Combine(fullDirectory, uniqueFileName);

        using (var fileStreamOut = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
        {
            await fileStream.CopyToAsync(fileStreamOut, cancellationToken);
        }

        var relativePath = Path.Combine(yearMonth, uniqueFileName);
        return relativePath;
    }

    public async Task<Stream> GetFileAsync(
        string filePath,
        CancellationToken cancellationToken = default
    )
    {
        var fullPath = Path.Combine(_baseDirectory, filePath);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        var memoryStream = new MemoryStream();
        using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
        {
            await fileStream.CopyToAsync(memoryStream, cancellationToken);
        }
        memoryStream.Position = 0;
        return memoryStream;
    }

    public Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_baseDirectory, filePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
        return Task.CompletedTask;
    }

    public Task<bool> FileExistsAsync(
        string filePath,
        CancellationToken cancellationToken = default
    )
    {
        var fullPath = Path.Combine(_baseDirectory, filePath);
        return Task.FromResult(File.Exists(fullPath));
    }
}

// Service to seed roles when the test host starts
internal class RoleSeederService : Microsoft.Extensions.Hosting.IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public RoleSeederService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var roleManager =
            scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<FalconDbContext>();

        // Ensure database is created
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        // Create roles if they don't exist
        string[] roleNames = { "Admin", "Teacher", "Student" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(
                    new Microsoft.AspNetCore.Identity.IdentityRole(roleName)
                );
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
