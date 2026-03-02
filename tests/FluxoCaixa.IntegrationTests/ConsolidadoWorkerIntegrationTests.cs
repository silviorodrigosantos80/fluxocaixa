using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using MassTransit.Testing;
using MediatR;

using FluxoCaixa.BuildingBlocks.Contracts.Events;
using FluxoCaixa.Consolidado.Infrastructure.Persistence;
using FluxoCaixa.Consolidado.Domain.Repositories;
using FluxoCaixa.Consolidado.Domain.Entities;

using FluxoCaixa.BuildingBlocks.Infrastructure.Persistence;
using FluxoCaixa.BuildingBlocks.Application.Abstractions;
using FluxoCaixa.BuildingBlocks.Domain.Enums;
using FluxoCaixa.Consolidado.Application.Commands;
using FluxoCaixa.Consolidado.Infrastructure.Messaging.Consumers;

namespace FluxoCaixa.IntegrationTests;

public class ConsolidadoWorkerIntegrationTests
{
    [Fact(DisplayName = "Deve atualizar saldo ao consumir evento de lançamento")]
    public async Task Deve_atualizar_saldo_ao_consumir_evento()
    {
        var services = new ServiceCollection();

        // ---------------------------
        // DbContext InMemory
        // ---------------------------
        services.AddDbContext<ConsolidadoDbContext>(opt =>
            opt.UseInMemoryDatabase("ConsolidadoDb"));

        services.AddScoped<IUnitOfWork, UnitOfWork<ConsolidadoDbContext>>();

        services.AddScoped<ISaldoDiarioRepository, SaldoDiarioRepository>();
        services.AddScoped<IProcessedEventRepository, ProcessedEventRepository>();

        // ---------------------------
        // MediatR
        // ---------------------------
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(
                typeof(ProcessarLancamentoIntegrationCommand).Assembly));

        // ---------------------------
        // MassTransit Test Harness
        // ---------------------------
        services.AddMassTransitTestHarness(cfg =>
        {
            cfg.AddConsumer<LancamentoCriadoConsumer>();
        });

        var provider = services.BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        var userId = Guid.NewGuid();
        var data = DateTime.UtcNow;

        // Act → Publica evento
        await harness.Bus.Publish(new LancamentoCriadoIntegrationEvent(
            Guid.NewGuid(),
            userId,
            Guid.NewGuid(),
            data.Date,
            150m,
            TipoLancamento.Credito,
            data.Date));

        // Aguarda processamento
        Assert.True(await harness.Consumed.Any<LancamentoCriadoIntegrationEvent>());

        using var scope = provider.CreateScope();
        var saldoDiario = scope.ServiceProvider.GetRequiredService<ISaldoDiarioRepository>();

        var saldo = await saldoDiario.GetAsync(userId, DateOnly.FromDateTime(data.Date), new());

        // Assert
        saldo.Should().NotBeNull();
        saldo!.TotalCredito.Should().Be(150m);
        saldo.Saldo.Should().Be(150m);

        await harness.Stop();
    }
}