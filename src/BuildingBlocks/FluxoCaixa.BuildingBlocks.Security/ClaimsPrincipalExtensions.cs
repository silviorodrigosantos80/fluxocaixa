using System.Security.Claims;

namespace FluxoCaixa.BuildingBlocks.Security;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue("sub");

        if (sub is null)
            throw new UnauthorizedAccessException("UserId claim not found.");

        return Guid.Parse(sub);
    }
}