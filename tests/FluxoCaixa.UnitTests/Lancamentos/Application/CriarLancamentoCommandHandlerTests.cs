using Xunit;
using Moq;
using FluentAssertions;
using FluxoCaixa.Lancamentos.Application.Commands;
using FluxoCaixa.BuildingBlocks.Application.Abstractions;
using FluxoCaixa.BuildingBlocks.Domain.Enums;
using FluxoCaixa.Lancamentos.Domain.Entities;
using FluxoCaixa.Lancamentos.Domain.Abstractions;
using FluxoCaixa.Lancamentos.Application.Commands.CriarLancamento;
using FluxoCaixa.BuildingBlocks.Domain.Exceptions;

namespace FluxoCaixa.UnitTests.Lancamentos.Application;

public class CriarLancamentoCommandHandlerTests
{
    private readonly Mock<ILancamentoRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CriarLancamentoCommandHandler _handler;

    public CriarLancamentoCommandHandlerTests()
    {
        _repositoryMock = new Mock<ILancamentoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new CriarLancamentoCommandHandler(
            _repositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact(DisplayName = "Deve criar e persistir lançamento")]
    public async Task Deve_criar_e_persistir_lancamento()
    {
        var userId = Guid.NewGuid();
        var data = DateTime.UtcNow;

        var command = new CriarLancamentoCommand(
            userId,
            data,
            150m,
            TipoLancamento.Credito);

        await _handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Lancamento>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Não deve chamar SaveChanges mais de uma vez")]
    public async Task Deve_chamar_SaveChanges_apenas_uma_vez()
    {
        var command = new CriarLancamentoCommand(
            Guid.NewGuid(),
            DateTime.UtcNow,
            100m,
            TipoLancamento.Credito);

        await _handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Não deve persistir lançamento se domínio lançar exceção")]
    public async Task Nao_deve_persistir_se_domino_lancar_excecao()
    {
        var command = new CriarLancamentoCommand(
            Guid.NewGuid(),
            DateTime.UtcNow,
            0m, // inválido
            TipoLancamento.Credito);

        Func<Task> act = async () =>
            await _handler.Handle(command, CancellationToken.None);

        await act.Should()
            .ThrowAsync<DomainException>()
            .WithMessage("Valor deve ser maior que zero*");

        _repositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Lancamento>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact(DisplayName = "Handler deve criar Aggregate com dados corretos")]
    public async Task Handler_deve_criar_aggregate_correto()
    {
        Lancamento? lancamentoCapturado = null;

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Lancamento>(), It.IsAny<CancellationToken>()))
            .Callback<Lancamento, CancellationToken>((l, _) => lancamentoCapturado = l)
            .Returns(Task.CompletedTask);

        var userId = Guid.NewGuid();
        var data = DateTime.UtcNow;

        var command = new CriarLancamentoCommand(
            userId,
            data.Date,
            300m,
            TipoLancamento.Debito);

        await _handler.Handle(command, CancellationToken.None);

        lancamentoCapturado.Should().NotBeNull();
        lancamentoCapturado!.UserId.Should().Be(userId);
        lancamentoCapturado.Valor.Amount.Should().Be(300m);
        lancamentoCapturado.Tipo.Should().Be(TipoLancamento.Debito);
        lancamentoCapturado.Data.Should().Be(data.Date);
    }
}