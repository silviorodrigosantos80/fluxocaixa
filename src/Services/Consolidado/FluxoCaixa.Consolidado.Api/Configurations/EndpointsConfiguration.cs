namespace FluxoCaixa.Consolidado.Api.Configurations;

public static class EndpointsConfiguration
{
    public static IServiceCollection AddEndpoints(
        this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}