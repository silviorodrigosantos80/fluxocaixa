using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

using FluxoCaixa.Lancamentos.Application.Commands.CriarLancamento;
using FluxoCaixa.Lancamentos.Infrastructure.Persistence;
using FluxoCaixa.Lancamentos.Domain.Abstractions;

using FluxoCaixa.BuildingBlocks.Infrastructure.Persistence;
using FluxoCaixa.BuildingBlocks.Application.Abstractions;
using FluxoCaixa.BuildingBlocks.Domain.Enums;

namespace FluxoCaixa.IntegrationTests;

public class ResilienciaIntegrationTests
{
    [Fact(DisplayName = "Lançamentos deve funcionar mesmo sem Consolidado")]
    public async Task Lancamentos_deve_funcionar_sem_consolidado()
    {
        var services = new ServiceCollection();

        // ---------------------------
        // Apenas Lançamentos
        // ---------------------------
        services.AddDbContext<LancamentosDbContext>(opt =>
            opt.UseInMemoryDatabase("LancamentosDb_Resiliencia"));

        services.AddScoped<IUnitOfWork, UnitOfWork<LancamentosDbContext>>();
        services.AddScoped<ILancamentoRepository, LancamentoRepository>();
        

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(
                typeof(CriarLancamentoCommand).Assembly));

        var provider = services.BuildServiceProvider(true);

        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var userId = Guid.NewGuid();
        var data = DateTime.UtcNow;

        // Act
        var act = async () =>
            await mediator.Send(new CriarLancamentoCommand(
                userId,
                data,
                200m,
                TipoLancamento.Credito));

        // Assert: não deve lançar exceção
        await act.Should().NotThrowAsync();

        var db = scope.ServiceProvider.GetRequiredService<LancamentosDbContext>();

        var outbox = await db.OutboxMessages.FirstOrDefaultAsync();

        outbox.Should().NotBeNull();
        outbox!.Type.Should().Contain("LancamentoCriado");
    }
}