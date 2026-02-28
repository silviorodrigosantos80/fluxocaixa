using FluxoCaixa.Lancamentos.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FluxoCaixa.Lancamentos.Api.Contracts.Requests;

public sealed class CriarLancamentoRequest
{
    [Required]
    public DateTime Data { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Valor { get; set; }

    [Required]
    public TipoLancamento Tipo { get; set; }
}