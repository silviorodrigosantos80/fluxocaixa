using FluxoCaixa.Consolidado.Application.Queries;
using FluxoCaixa.BuildingBlocks.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FluxoCaixa.Consolidado.Api.Controllers;

[Authorize(Roles = "comerciante")]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class ConsolidadoController : ControllerBase
{
    private readonly IMediator _mediator;

    public ConsolidadoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtém o saldo diário do usuário autenticado.
    /// </summary>
    /// <remarks>
    /// A data deve ser informada no formato **YYYY-MM-DD**.
    /// 
    /// Exemplo:
    /// 
    ///     GET /api/consolidado?data=2026-03-01
    /// 
    /// </remarks>
    /// <param name="data">
    /// Data no formato obrigatório YYYY-MM-DD.
    /// </param>
    /// <response code="200">Saldo encontrado com sucesso.</response>
    /// <response code="404">Saldo não encontrado para a data informada.</response>
    /// <response code="400">Data inválida.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="403">Usuário sem permissão.</response>
    [HttpGet]
    [ProducesResponseType(typeof(SaldoDiarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Get(
        [FromQuery]
        [BindRequired]
        DateOnly data,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var query = new ObterSaldoDiarioQuery(userId, data);

        var result = await _mediator.Send(query, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}