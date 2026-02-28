using Microsoft.AspNetCore.Builder;

namespace FluxoCaixa.BuildingBlocks.Web.Extensions;

public static class WebApplicationExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<
            FluxoCaixa.BuildingBlocks.Web.Middlewares.ExceptionHandlingMiddleware>();
    }
}