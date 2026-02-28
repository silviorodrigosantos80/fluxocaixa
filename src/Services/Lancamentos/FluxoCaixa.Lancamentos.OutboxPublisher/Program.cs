using FluxoCaixa.Lancamentos.Infrastructure.Persistence;
using FluxoCaixa.Lancamentos.OutboxPublisher.Configurations;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<LancamentosDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Database")));

builder.Services.AddMessaging(builder.Configuration);

builder.Services.AddHostedService<OutboxPublisherWorker>();

var host = builder.Build();
host.Run();
