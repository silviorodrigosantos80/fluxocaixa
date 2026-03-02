using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using MassTransit.Testing;
using MediatR;

using FluxoCaixa.BuildingBlocks.Contracts.Events;
using FluxoCaixa.Consolidado.Infrastructure.Messaging.Consumers;
using FluxoCaixa.Consolidado.Infrastructure.Persistence;
using FluxoCaixa.Consolidado.Domain.Repositories;
using FluxoCaixa.Consolidado.Domain.Entities;

using FluxoCaixa.BuildingBlocks.Infrastructure.Persistence;
using FluxoCaixa.BuildingBlocks.Application.Abstractions;
using FluxoCaixa.BuildingBlocks.Domain.Enums;
using Microsoft.Extensions.Logging;
using FluxoCaixa.Consolidado.Application.Commands;

namespace FluxoCaixa.IntegrationTests;

public class IdempotenciaIntegrationTests
{
    [Fact(DisplayName = "Não deve duplicar saldo ao receber evento duplicado")]
    public async Task Nao_deve_duplicar_saldo_quando_evento_repetido()
    {
        var services = new ServiceCollection();

        services.AddLogging(logging => logging.ClearProviders());

        services.AddDbContext<ConsolidadoDbContext>(opt =>
            opt.UseInMemoryDatabase("ConsolidadoDb_Idempotencia"));

        services.AddScoped<IUnitOfWork, UnitOfWork<ConsolidadoDbContext>>();
        services.AddScoped<ISaldoDiarioRepository, SaldoDiarioRepository>();
        services.AddScoped<IProcessedEventRepository, ProcessedEventRepository>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(
                typeof(ProcessarLancamentoIntegrationCommand).Assembly));

        services.AddMassTransitTestHarness(cfg =>
        {
            cfg.AddConsumer<LancamentoCriadoConsumer>();
        });

        var provider = services.BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var lancamentoId = Guid.NewGuid();
        var data = DateTime.UtcNow;

        var evento = new LancamentoCriadoIntegrationEvent(
            eventId,
            userId,
            lancamentoId,
            data,
            100m,
            TipoLancamento.Credito,
            data);

        // Publica duas vezes o mesmo evento
        await harness.Bus.Publish(evento);
        await harness.Bus.Publish(evento);

        Assert.True(await harness.Consumed.Any<LancamentoCriadoIntegrationEvent>());

        using var scope = provider.CreateScope();
        var saldoDiarioRepository = scope.ServiceProvider.GetRequiredService<ISaldoDiarioRepository>();

        var saldo = await saldoDiarioRepository.GetAsync(userId, DateOnly.FromDateTime(data.Date), new());

        var processedEventRepository = scope.ServiceProvider.GetRequiredService<IProcessedEventRepository>();
        var processedExists = await processedEventRepository.ExistsAsync(evento.EventId, new());

        // Saldo deve ser aplicado apenas uma vez
        saldo.Should().NotBeNull();
        saldo!.TotalCredito.Should().Be(100m);
        saldo.Saldo.Should().Be(100m);

        // ProcessedEvent deve existir
        processedExists.Should().Be(true);

        await harness.Stop();
    }
}