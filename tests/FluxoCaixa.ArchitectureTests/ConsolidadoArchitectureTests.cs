using Xunit;
using NetArchTest.Rules;

namespace FluxoCaixa.ArchitectureTests.Architecture;

public class ConsolidadoArchitectureTests
{
    [Fact(DisplayName = "Domain não deve depender de Infrastructure")]
    public void Domain_nao_deve_depender_de_Infrastructure()
    {
        var result = Types.InAssembly(
                typeof(FluxoCaixa.Consolidado.Domain.Entities.SaldoDiario).Assembly)
            .Should()
            .NotHaveDependencyOn("FluxoCaixa.Consolidado.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact(DisplayName = "Application não deve depender de Api")]
    public void Application_nao_deve_depender_de_Api()
    {
        var result = Types.InAssembly(
                typeof(FluxoCaixa.Consolidado.Application.Commands.ProcessarLancamentoIntegrationCommand).Assembly)
            .Should()
            .NotHaveDependencyOn("FluxoCaixa.Consolidado.Api")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact(DisplayName = "Infrastructure não deve depender de Api")]
    public void Infrastructure_nao_deve_depender_de_Api()
    {
        var result = Types.InAssembly(
                typeof(FluxoCaixa.Consolidado.Infrastructure.Persistence.ConsolidadoDbContext).Assembly)
            .Should()
            .NotHaveDependencyOn("FluxoCaixa.Consolidado.Api")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}