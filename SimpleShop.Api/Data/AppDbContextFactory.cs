using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SimpleShop.Api.Data;

/// <summary>
/// Provides a design-time factory for creating <see cref="AppDbContext"/> instances.
/// Used by Entity Framework Core tools for migrations, updates,
/// and migration bundle generation outside runtime dependency injection.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    /// <summary>
    /// Creates a new <see cref="AppDbContext"/> instance for design-time operations.
    /// The connection string is resolved in the following priority order:
    /// 1. Command-line argument (--connection)
    /// 2. Environment variable (CONNECTION_STRING)
    /// 3. appsettings.json / appsettings.Development.json
    /// </summary>
    /// <param name="args">
    /// Command-line arguments passed by Entity Framework tooling.
    /// </param>
    /// <returns>
    /// A configured <see cref="AppDbContext"/> instance.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no valid connection string is found.
    /// </exception>
    public AppDbContext CreateDbContext(string[] args)
    {
        string? connectionString = null;

        #region Read CLI Argument

        // Allows explicit connection string injection:
        // --connection "Server=...;Database=..."
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--connection")
            {
                connectionString = args[i + 1];
                break;
            }
        }

        #endregion

        #region Read Environment Variable

        // Useful for CI/CD pipelines and deployment servers.
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        }

        #endregion

        #region Read Configuration Files

        // Fallback to local configuration files for development usage.
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #endregion

        #region Validate Result

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string not provided.");
        }

        #endregion

        #region Build DbContext

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseMySql(
            connectionString,
            new MySqlServerVersion(new Version(8, 0, 36)));

        return new AppDbContext(optionsBuilder.Options);

        #endregion
    }
}