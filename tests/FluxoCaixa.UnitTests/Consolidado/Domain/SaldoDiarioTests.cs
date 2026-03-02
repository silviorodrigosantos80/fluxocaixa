using Xunit;
using FluentAssertions;
using FluxoCaixa.Consolidado.Domain.Entities;

namespace FluxoCaixa.UnitTests.Consolidado.Domain;

public class SaldoDiarioTests
{
    [Fact(DisplayName = "Deve criar saldo diário com valores iniciais zerados")]
    public void Deve_criar_saldo_com_valores_iniciais_zerados()
    {
        var userId = Guid.NewGuid();
        var data = DateOnly.FromDateTime(DateTime.UtcNow);

        var saldo = new SaldoDiario(userId, data);

        saldo.TotalCredito.Should().Be(0);
        saldo.TotalDebito.Should().Be(0);
        saldo.Saldo.Should().Be(0);
    }

    [Fact(DisplayName = "Deve aplicar crédito corretamente")]
    public void Deve_aplicar_credito()
    {
        var saldo = CriarSaldo();

        saldo.AplicarCredito(100);

        saldo.TotalCredito.Should().Be(100);
        saldo.Saldo.Should().Be(100);
    }

    [Fact(DisplayName = "Deve aplicar débito corretamente")]
    public void Deve_aplicar_debito()
    {
        var saldo = CriarSaldo();

        saldo.AplicarCredito(200);
        saldo.AplicarDebito(50);

        saldo.TotalDebito.Should().Be(50);
        saldo.Saldo.Should().Be(150);
    }

    [Fact(DisplayName = "Deve acumular múltiplos créditos")]
    public void Deve_acumular_multiplos_creditos()
    {
        var saldo = CriarSaldo();

        saldo.AplicarCredito(100);
        saldo.AplicarCredito(50);
        saldo.AplicarCredito(25);

        saldo.TotalCredito.Should().Be(175);
        saldo.Saldo.Should().Be(175);
    }

    [Fact(DisplayName = "Deve acumular múltiplos débitos")]
    public void Deve_acumular_multiplos_debitos()
    {
        var saldo = CriarSaldo();

        saldo.AplicarCredito(300);
        saldo.AplicarDebito(50);
        saldo.AplicarDebito(25);

        saldo.TotalDebito.Should().Be(75);
        saldo.Saldo.Should().Be(225);
    }

    [Fact(DisplayName = "Saldo deve refletir corretamente crédito menos débito")]
    public void Saldo_deve_ser_credito_menos_debito()
    {
        var saldo = CriarSaldo();

        saldo.AplicarCredito(500);
        saldo.AplicarDebito(120);

        saldo.Saldo.Should().Be(380);
    }

    [Fact(DisplayName = "Não deve permitir aplicar crédito com valor negativo")]
    public void Nao_deve_permitir_credito_negativo()
    {
        var saldo = CriarSaldo();

        Action act = () => saldo.AplicarCredito(-10);

        act.Should().Throw<ArgumentException>();
    }

    [Fact(DisplayName = "Não deve permitir aplicar débito com valor negativo")]
    public void Nao_deve_permitir_debito_negativo()
    {
        var saldo = CriarSaldo();

        Action act = () => saldo.AplicarDebito(-20);

        act.Should().Throw<ArgumentException>();
    }

    // Método auxiliar para manter testes limpos
    private static SaldoDiario CriarSaldo()
    {
        return new SaldoDiario(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.UtcNow));
    }
}