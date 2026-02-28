using FluxoCaixa.BuildingBlocks.Infrastructure.Extensions;
using FluxoCaixa.Lancamentos.Api.Configurations;
using FluxoCaixa.Lancamentos.Application.Commands.CriarLancamento;
using FluxoCaixa.Lancamentos.Infrastructure.Persistence;
using FluxoCaixa.BuildingBlocks.Web.Extensions;
using FluxoCaixa.BuildingBlocks.Security;
using FluxoCaixa.Lancamentos.Domain.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddPostgres<LancamentosDbContext>(builder.Configuration, "LancamentosConnection");


//Aqui posso separar para não popular muito a Program e separar esta responsabilidade
// Vou deixar assim por enquanto porque o projeto é pequeno mas se for evoluir, separo
builder.Services.AddScoped<ILancamentoRepository, LancamentoRepository>();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(CriarLancamentoCommandHandler).Assembly));
//====================================================================================

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddSwaggerConfiguration();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseGlobalExceptionHandling();
app.UseSwaggerConfiguration();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//Eu poderia utilizar Minimal API aqui mas vou deixar em Controller, é adequado 
// a arquitetura que estamos montando e já deixa pronto para possível crescimento. 
app.MapControllers();

app.Run();
