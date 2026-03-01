using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FluxoCaixa.Consolidado.Infrastructure.Persistence;

public sealed class ConsolidadoDbContextFactory 
    : IDesignTimeDbContextFactory<ConsolidadoDbContext>
{
    public ConsolidadoDbContext CreateDbContext(string[] args)
    {
        var environment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? "Development";

        var basePath = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json",
                optional: false)
            .AddJsonFile($"appsettings.{environment}.json",
                optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration
            .GetConnectionString("ConsolidadoConnection");

        var optionsBuilder = new DbContextOptionsBuilder<ConsolidadoDbContext>();

        optionsBuilder.UseNpgsql(connectionString);

        return new ConsolidadoDbContext(optionsBuilder.Options);
    }
}