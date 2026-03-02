using Xunit;
using NetArchTest.Rules;
using FluxoCaixa.Lancamentos.Application.Commands.CriarLancamento;

namespace FluxoCaixa.ArchitectureTests.Architecture;

public class LancamentosArchitectureTests
{
    [Fact(DisplayName = "Domain não deve depender de Infrastructure")]
    public void Domain_nao_deve_depender_de_Infrastructure()
    {
        var result = Types.InAssembly(
                typeof(FluxoCaixa.Lancamentos.Domain.Entities.Lancamento).Assembly)
            .Should()
            .NotHaveDependencyOn("FluxoCaixa.Lancamentos.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact(DisplayName = "Domain não deve depender de Application")]
    public void Domain_nao_deve_depender_de_Application()
    {
        var result = Types.InAssembly(
                typeof(FluxoCaixa.Lancamentos.Domain.Entities.Lancamento).Assembly)
            .Should()
            .NotHaveDependencyOn("FluxoCaixa.Lancamentos.Application")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact(DisplayName = "Application não deve depender de Api")]
    public void Application_nao_deve_depender_de_Api()
    {
        var result = Types.InAssembly(
                typeof(CriarLancamentoCommand).Assembly)
            .Should()
            .NotHaveDependencyOn("FluxoCaixa.Lancamentos.Api")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact(DisplayName = "Infrastructure não deve depender de Api")]
    public void Infrastructure_nao_deve_depender_de_Api()
    {
        var result = Types.InAssembly(
                typeof(FluxoCaixa.Lancamentos.Infrastructure.Persistence.LancamentosDbContext).Assembly)
            .Should()
            .NotHaveDependencyOn("FluxoCaixa.Lancamentos.Api")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}