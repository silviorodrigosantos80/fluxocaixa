using System.ComponentModel.DataAnnotations;

namespace FluxoCaixa.BuildingBlocks.Security;

public sealed class JwtOptions
{
    public const string SectionName = "Authentication";

    [Required]
    public string Authority { get; init; } = null!;

    [Required]
    public string Audience { get; init; } = null!;

    public bool RequireHttpsMetadata { get; init; }
}