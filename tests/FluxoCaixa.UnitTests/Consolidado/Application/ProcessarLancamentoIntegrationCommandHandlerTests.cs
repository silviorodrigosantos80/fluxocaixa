using Xunit;
using Moq;
using FluentAssertions;
using FluxoCaixa.Consolidado.Application.Commands;
using FluxoCaixa.Consolidado.Domain.Repositories;
using FluxoCaixa.BuildingBlocks.Application.Abstractions;
using FluxoCaixa.Consolidado.Domain.Entities;
using FluxoCaixa.BuildingBlocks.Domain.Enums;
using Xunit.Abstractions;

namespace FluxoCaixa.UnitTests.Consolidado.Application.Commands;

public class ProcessarLancamentoIntegrationCommandHandlerTests
{
    private readonly Mock<ISaldoDiarioRepository> _saldoRepositoryMock;
    private readonly Mock<IProcessedEventRepository> _processedRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ProcessarLancamentoIntegrationCommandHandler _handler;
    private readonly ITestOutputHelper _output;

    public ProcessarLancamentoIntegrationCommandHandlerTests(ITestOutputHelper output)
    {
        _saldoRepositoryMock = new Mock<ISaldoDiarioRepository>();
        _processedRepositoryMock = new Mock<IProcessedEventRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _output = output;

        _handler = new ProcessarLancamentoIntegrationCommandHandler(
            _saldoRepositoryMock.Object,
            _processedRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact(DisplayName = "Deve criar instância do handler corretamente")]
    public void Deve_criar_instancia_do_handler()
    {
        _output.WriteLine("Validando criação do handler...");

        _handler.Should().NotBeNull();
    }

    [Fact(DisplayName = "Não deve processar evento já processado (Idempotência)")]
    public async Task Nao_deve_processar_se_evento_ja_foi_processado()
    {
        _output.WriteLine("Validando comportamento de idempotência...");

        var command = new ProcessarLancamentoIntegrationCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.Date,
            100,
            TipoLancamento.Credito);

        _processedRepositoryMock
            .Setup(x => x.ExistsAsync(command.EventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        _saldoRepositoryMock.Verify(
            x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact(DisplayName = "Deve somar valor ao saldo quando lançamento for crédito")]
    public async Task Deve_somar_valor_ao_saldo_quando_for_credito()
    {
        _output.WriteLine("Validando aplicação de crédito no saldo...");

        var eventId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var data = DateTime.UtcNow.Date;

        var command = new ProcessarLancamentoIntegrationCommand(
            eventId,
            userId,
            data,
            50,
            TipoLancamento.Credito);

        var saldoExistente = new SaldoDiario(userId, DateOnly.FromDateTime(data));
        saldoExistente.AplicarCredito(100);

        _processedRepositoryMock
            .Setup(x => x.ExistsAsync(eventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _saldoRepositoryMock
            .Setup(x => x.GetAsync(userId, DateOnly.FromDateTime(data), It.IsAny<CancellationToken>()))
            .ReturnsAsync(saldoExistente);

        await _handler.Handle(command, CancellationToken.None);

        saldoExistente.Saldo.Should().Be(150);

        _processedRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<ProcessedEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Deve subtrair valor do saldo quando lançamento for débito")]
    public async Task Deve_subtrair_valor_do_saldo_quando_for_debito()
    {
        _output.WriteLine("Validando aplicação de débito no saldo...");

        var eventId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var data = DateTime.UtcNow.Date;

        var command = new ProcessarLancamentoIntegrationCommand(
            eventId,
            userId,
            data,
            30,
            TipoLancamento.Debito);

        var saldoExistente = new SaldoDiario(userId, DateOnly.FromDateTime(data));
        saldoExistente.AplicarCredito(100);

        _processedRepositoryMock
            .Setup(x => x.ExistsAsync(eventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _saldoRepositoryMock
            .Setup(x => x.GetAsync(userId, DateOnly.FromDateTime(data), It.IsAny<CancellationToken>()))
            .ReturnsAsync(saldoExistente);

        await _handler.Handle(command, CancellationToken.None);

        saldoExistente.Saldo.Should().Be(70);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Deve criar saldo quando não existir registro para o dia")]
    public async Task Deve_criar_saldo_se_nao_existir()
    {
        _output.WriteLine("Validando criação de novo saldo diário...");

        var eventId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var data = DateTime.UtcNow.Date;

        var command = new ProcessarLancamentoIntegrationCommand(
            eventId,
            userId,
            data,
            200,
            TipoLancamento.Credito);

        _processedRepositoryMock
            .Setup(x => x.ExistsAsync(eventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _saldoRepositoryMock
            .Setup(x => x.GetAsync(userId, DateOnly.FromDateTime(data), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SaldoDiario?)null);

        await _handler.Handle(command, CancellationToken.None);

        _saldoRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<SaldoDiario>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Não deve chamar SaveChanges mais de uma vez por processamento")]
    public async Task Deve_chamar_SaveChanges_apenas_uma_vez()
    {
        _output.WriteLine("Validando consistência transacional...");

        var eventId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var data = DateTime.UtcNow.Date;

        var command = new ProcessarLancamentoIntegrationCommand(
            eventId,
            userId,
            data,
            10,
            TipoLancamento.Credito);

        var saldoExistente = new SaldoDiario(userId, DateOnly.FromDateTime(data));

        _processedRepositoryMock
            .Setup(x => x.ExistsAsync(eventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _saldoRepositoryMock
            .Setup(x => x.GetAsync(userId, DateOnly.FromDateTime(data), It.IsAny<CancellationToken>()))
            .ReturnsAsync(saldoExistente);

        await _handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}