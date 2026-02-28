using MediatR;

namespace FluxoCaixa.BuildingBlocks.Application.Abstractions;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}