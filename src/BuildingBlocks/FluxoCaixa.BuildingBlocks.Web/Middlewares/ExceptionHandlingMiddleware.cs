using FluxoCaixa.BuildingBlocks.Application.Results;
using FluxoCaixa.BuildingBlocks.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FluxoCaixa.BuildingBlocks.Web.Middlewares;
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            await HandleExceptionAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Validation",
                ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado");

            await HandleExceptionAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Failure",
                "Ocorreu um erro inesperado.");
        }
    }

    private static Task HandleExceptionAsync(
        HttpContext context,
        int statusCode,
        string type,
        string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var problemDetails = new
        {
            type,
            title = statusCode == 400 ? "Erro de validação" : "Erro interno",
            status = statusCode,
            detail = message
        };

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(problemDetails));
    }
}