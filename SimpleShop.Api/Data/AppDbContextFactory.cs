using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SimpleShop.Api.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        string? connectionString = null;

        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--connection")
            {
                connectionString = args[i + 1];
                break;
            }
        }

        // Fallback uden hardcoded password (kun til lokal dev)
        connectionString ??= Environment.GetEnvironmentVariable("CONNECTION_STRING")
            ?? throw new InvalidOperationException("Connection string not provided.");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseMySql(
            connectionString,
            new MySqlServerVersion(new Version(8, 0, 36)));

        return new AppDbContext(optionsBuilder.Options);
    }
}