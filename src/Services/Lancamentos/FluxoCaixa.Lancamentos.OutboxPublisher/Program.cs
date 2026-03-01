using FluxoCaixa.Lancamentos.Infrastructure.Persistence;
using FluxoCaixa.Lancamentos.OutboxPublisher.Configurations;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddMessaging(builder.Configuration);

builder.Services.AddHostedService<OutboxPublisherWorker>();

var host = builder.Build();
host.Run();
