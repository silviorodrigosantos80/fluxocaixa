using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FluxoCaixa.Lancamentos.Infrastructure.Persistence;

public sealed class LancamentosDbContextFactory 
    : IDesignTimeDbContextFactory<LancamentosDbContext>
{
    public LancamentosDbContext CreateDbContext(string[] args)
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
            .GetConnectionString("LancamentosConnection");

        var optionsBuilder = new DbContextOptionsBuilder<LancamentosDbContext>();

        optionsBuilder.UseNpgsql(connectionString);

        return new LancamentosDbContext(optionsBuilder.Options);
    }
}