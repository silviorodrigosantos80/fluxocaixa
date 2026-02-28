using System.ComponentModel.DataAnnotations;

namespace FluxoCaixa.Lancamentos.OutboxPublisher.Configurations;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMQ";

    [Required(ErrorMessage = "RabbitMQ Host é obrigatório.")]
    [MinLength(3, ErrorMessage = "RabbitMQ Host inválido.")]
    public string Host { get; init; } = null!;

    [Required(ErrorMessage = "RabbitMQ Username é obrigatório.")]
    [MinLength(1)]
    public string Username { get; init; } = null!;

    [Required(ErrorMessage = "RabbitMQ Password é obrigatório.")]
    [MinLength(1)]
    public string Password { get; init; } = null!;
}