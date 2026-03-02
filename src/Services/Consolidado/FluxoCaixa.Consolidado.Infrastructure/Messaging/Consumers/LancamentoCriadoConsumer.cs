using FluxoCaixa.BuildingBlocks.Contracts.Events;
using FluxoCaixa.Consolidado.Application.Commands;
using MassTransit;
using MediatR;

namespace FluxoCaixa.Consolidado.Infrastructure.Messaging.Consumers;

public sealed class LancamentoCriadoConsumer
    : IConsumer<LancamentoCriadoIntegrationEvent>
{
    private readonly IMediator _mediator;

    public LancamentoCriadoConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(
        ConsumeContext<LancamentoCriadoIntegrationEvent> context)
    {
        var message = context.Message;

        var command = new ProcessarLancamentoIntegrationCommand(
            message.EventId,
            message.UserId,
            message.Data,
            message.Valor,
            message.Tipo);

        await _mediator.Send(command);
    }
}