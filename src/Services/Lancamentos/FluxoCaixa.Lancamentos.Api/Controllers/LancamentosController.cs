using FluxoCaixa.BuildingBlocks.Security;
using FluxoCaixa.Lancamentos.Api.Contracts.Requests;
using FluxoCaixa.Lancamentos.Application.Commands.CriarLancamento;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FluxoCaixa.Lancamentos.Api.Controllers;

[Authorize(Roles = "comerciante")]
[ApiController]
[Route("api/lancamentos")]
[Produces("application/json")]
public class LancamentosController : ControllerBase
{
    private readonly IMediator _mediator;

    public LancamentosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Cria um novo lançamento (crédito ou débito).
    /// </summary>
    /// <remarks>
    /// O usuário autenticado será automaticamente associado ao lançamento.
    /// </remarks>
    /// <response code="201">Lançamento criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Não autenticado</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Criar(
        [FromBody] CriarLancamentoRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        
        var command = new CriarLancamentoCommand(
            userId,
            request.Data,
            request.Valor,
            request.Tipo);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { erro = result.Error.Description });

        return CreatedAtAction(
            nameof(Criar),
            new { id = result.Value },
            result.Value);
    }
}