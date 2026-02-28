using MediatR;

namespace FluxoCaixa.BuildingBlocks.Application.Abstractions;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}