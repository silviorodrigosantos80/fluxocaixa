using FluxoCaixa.BuildingBlocks.Infrastructure.Messaging;
using FluxoCaixa.Consolidado.Worker.Configurations;

var builder = Host.CreateApplicationBuilder(args);

// Persistência
builder.Services.AddPersistence(builder.Configuration);

// Application
builder.Services.AddApplication();

// Mensageria
builder.Services.AddMessaging(builder.Configuration);

var host = builder.Build();
host.Run();
