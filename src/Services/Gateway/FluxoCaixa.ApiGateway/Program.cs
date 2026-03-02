using FluxoCaixa.BuildingBlocks.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

builder.Services
        .AddOptions<JwtOptions>()
        .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
        .ValidateDataAnnotations()
        .ValidateOnStart();

var jwtOptions = builder.Configuration
    .GetSection(JwtOptions.SectionName)
    .Get<JwtOptions>()!;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = jwtOptions.Authority;
        options.RequireHttpsMetadata = jwtOptions.RequireHttpsMetadata;
        options.Audience = jwtOptions.Audience;
    });

builder.Services.AddAuthorization();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FluxoCaixa Gateway",
        Version = "v1"
    });

    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{jwtOptions.Authority}/protocol/openid-connect/auth"),
                TokenUrl = new Uri($"{jwtOptions.Authority}/protocol/openid-connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID Connect scope" }
                }
            }
        }
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { "openid" }
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway v1");
    options.OAuthClientId("fluxocaixa-gateway");
    options.OAuthUsePkce();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy().RequireAuthorization();

app.Run();