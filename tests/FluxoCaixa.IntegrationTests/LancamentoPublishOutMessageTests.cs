using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MassTransit.Testing;
using MediatR;

using FluxoCaixa.Lancamentos.Application.Commands.CriarLancamento;
using FluxoCaixa.Lancamentos.Infrastructure.Persistence;
using FluxoCaixa.Lancamentos.Domain.Abstractions;
using FluxoCaixa.Lancamentos.Domain.Events;

using FluxoCaixa.Consolidado.Infrastructure.Persistence;
using FluxoCaixa.Consolidado.Domain.Repositories;

using FluxoCaixa.BuildingBlocks.Infrastructure.Persistence;
using FluxoCaixa.BuildingBlocks.Application.Abstractions;
using FluxoCaixa.BuildingBlocks.Domain.Enums;
using MassTransit;

namespace FluxoCaixa.IntegrationTests;

public class LancamentoPublishOutMessageTests
{
    [Fact(DisplayName = "Deve processar fluxo completo de lançamento até consolidado")]
    public async Task Deve_processar_fluxo_completo()
    {
        var services = new ServiceCollection();

        // ---------------------------
        // DbContexts InMemory
        // ---------------------------
        services.AddDbContext<LancamentosDbContext>(opt =>
            opt.UseInMemoryDatabase("LancamentosDb"));

        services.AddDbContext<ConsolidadoDbContext>(opt =>
            opt.UseInMemoryDatabase("ConsolidadoDb"));

        // ---------------------------
        // UnitOfWork (igual AddDbPostgres faria)
        // ---------------------------
        services.AddScoped<IUnitOfWork, UnitOfWork<LancamentosDbContext>>();

        // ---------------------------
        // Repositories reais
        // ---------------------------
        services.AddScoped<ILancamentoRepository, LancamentoRepository>();
        services.AddScoped<ISaldoDiarioRepository, SaldoDiarioRepository>();
        services.AddScoped<IProcessedEventRepository, ProcessedEventRepository>();

        // ---------------------------
        // MediatR
        // ---------------------------
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(
                typeof(CriarLancamentoCommand).Assembly));

        // ---------------------------
        // MassTransit Test Harness
        // ---------------------------
        services.AddMassTransitTestHarness();

        var provider = services.BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        using var scope = provider.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var userId = Guid.NewGuid();
        var data = DateTime.UtcNow;

        // Act
        await mediator.Send(new CriarLancamentoCommand(
            userId,
            data,
            100m,
            TipoLancamento.Credito));

        // Assert
        var lancamentosDb = scope.ServiceProvider
            .GetRequiredService<LancamentosDbContext>();

        var outboxMessage = await lancamentosDb.OutboxMessages.FirstOrDefaultAsync();

        outboxMessage.Should().NotBeNull();
        outboxMessage!.Type.Should()
            .Be(typeof(LancamentoCriadoDomainEvent).FullName);

        await harness.Stop();
    }
}