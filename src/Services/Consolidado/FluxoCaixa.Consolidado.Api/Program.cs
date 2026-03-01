using FluxoCaixa.BuildingBlocks.Security;
using FluxoCaixa.BuildingBlocks.Web.Extensions;
using FluxoCaixa.Consolidado.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Autenticação
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

// Persistência
builder.Services.AddPersistence(builder.Configuration);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

// Middleware
app.UseGlobalExceptionHandling();

app.UseSwaggerConfiguration(app.Environment);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();