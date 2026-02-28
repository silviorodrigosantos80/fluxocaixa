using FluxoCaixa.BuildingBlocks.Security;
using FluxoCaixa.Consolidado.Api.Configurations;
using FluxoCaixa.BuildingBlocks.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddJwtAuthentication(builder.Configuration);
    
builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddAuthorization();
builder.Services.AddSwaggerConfiguration();

builder.Services.AddApplication();
builder.Services.AddMessaging(builder.Configuration);
builder.Services.AddEndpoints();

var app = builder.Build();

app.UseGlobalExceptionHandling();
app.UseSwaggerConfiguration();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
