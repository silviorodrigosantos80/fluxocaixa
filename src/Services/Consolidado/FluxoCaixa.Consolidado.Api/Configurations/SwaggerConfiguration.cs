using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;

namespace FluxoCaixa.Consolidado.Api.Configurations;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerConfiguration(
        this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Informe o token JWT."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
            
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "FluxoCaixa - Serviço de Relatório Consolidado",
                Version = "v1",
                Description = "Serviço responsável pela consulta de saldo financeiro diário.",
                Contact = new OpenApiContact
                {
                    Name = "Silvio Santos",
                    Email = "silviorodrigosantos80@gmail.com",
                    Url = new Uri("https://github.com/silviorodrigosantos80")
                },
            });

            // Exibir enums como string, não vou usar String no Domain para não misturar o conceito de apresentação e domínio
            options.UseInlineDefinitionsForEnums();
        });

        // Converter enums para string no JSON
        services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerConfiguration(
        this IApplicationBuilder app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "FluxoCaixa Rel. Consolidado v1");
            options.RoutePrefix = string.Empty; // Swagger na raiz
        });

        return app;
    }
}