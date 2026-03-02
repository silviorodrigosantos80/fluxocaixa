using Xunit;
using FluentAssertions;
using FluxoCaixa.Lancamentos.Domain.Entities;
using FluxoCaixa.BuildingBlocks.Domain.Enums;
using FluxoCaixa.BuildingBlocks.Domain.Exceptions;

namespace FluxoCaixa.UnitTests.Lancamentos.Domain;

public class LancamentoTests
{
    [Fact(DisplayName = "Deve criar lançamento válido")]
    public void Deve_criar_lancamento_valido()
    {
        var userId = Guid.NewGuid();
        var data = DateTime.UtcNow.Date;
        var valor = 100m;
        var tipo = TipoLancamento.Credito;

        var lancamento = Lancamento.Criar(userId, data, valor, tipo);

        lancamento.Should().NotBeNull();
        lancamento.UserId.Should().Be(userId);
        lancamento.Data.Should().Be(data);

        // Ajuste aqui: Valor é ValueObject
        lancamento.Valor.Amount.Should().Be(valor);

        lancamento.Tipo.Should().Be(tipo);
    }

    [Fact(DisplayName = "Não deve permitir valor menor ou igual a zero")]
    public void Nao_deve_permitir_valor_zero()
    {
        var userId = Guid.NewGuid();
        var data = DateTime.UtcNow.Date;

        Action act = () =>
            Lancamento.Criar(userId, data, 0, TipoLancamento.Credito);

        act.Should().Throw<DomainException>()
           .WithMessage("Valor deve ser maior que zero*");
    }

    [Fact(DisplayName = "Não deve permitir valor negativo")]
    public void Nao_deve_permitir_valor_negativo()
    {
        var userId = Guid.NewGuid();
        var data = DateTime.UtcNow.Date;

        Action act = () =>
            Lancamento.Criar(userId, data, -10, TipoLancamento.Credito);

        act.Should().Throw<DomainException>()
           .WithMessage("Valor deve ser maior que zero*");
    }

    [Fact(DisplayName = "Deve gerar DomainEvent ao criar lançamento")]
    public void Deve_gerar_domain_event()
    {
        var userId = Guid.NewGuid();
        var data = DateTime.UtcNow.Date;

        var lancamento = Lancamento.Criar(userId, data, 50, TipoLancamento.Credito);

        lancamento.DomainEvents.Should().HaveCount(1);
        lancamento.DomainEvents.First().GetType().Name
            .Should().Contain("LancamentoCriado");
    }

    [Fact(DisplayName = "DomainEvent deve conter dados consistentes")]
    public void DomainEvent_deve_conter_dados_corretos()
    {
        var userId = Guid.NewGuid();
        var data = DateTime.UtcNow.Date;
        var valor = 200m;
        var tipo = TipoLancamento.Debito;

        var lancamento = Lancamento.Criar(userId, data, valor, tipo);

        var domainEvent = lancamento.DomainEvents.First();

        domainEvent.Should().NotBeNull();
    }

    [Fact(DisplayName = "Data informada deve ser preservada no lançamento")]
    public void Data_informada_deve_ser_preservada()
    {
        var userId = Guid.NewGuid();
        var data = DateTime.UtcNow.Date;

        var lancamento = Lancamento.Criar(userId, data, 10, TipoLancamento.Credito);

        lancamento.Data.Should().Be(data);
    }
}