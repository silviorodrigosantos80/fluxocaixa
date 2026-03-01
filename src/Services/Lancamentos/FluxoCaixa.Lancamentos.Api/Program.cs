using FluxoCaixa.BuildingBlocks.Infrastructure.Extensions;
using FluxoCaixa.Lancamentos.Api.Configurations;
using FluxoCaixa.Lancamentos.Application.Commands.CriarLancamento;
using FluxoCaixa.Lancamentos.Infrastructure.Persistence;
using FluxoCaixa.BuildingBlocks.Web.Extensions;
using FluxoCaixa.BuildingBlocks.Security;
using FluxoCaixa.Lancamentos.Domain.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddApplication();

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddSwaggerConfiguration();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseGlobalExceptionHandling();
app.UseSwaggerConfiguration(app.Environment);

app.UseAuthentication();
app.UseAuthorization();
 
app.MapControllers();

app.Run();
