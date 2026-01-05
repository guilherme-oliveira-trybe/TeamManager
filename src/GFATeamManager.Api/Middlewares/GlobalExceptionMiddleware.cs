using System.Net;
using System.Text.Json;

namespace GFATeamManager.Api.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = _environment.IsDevelopment()
            ? new ErrorResponse
            {
                StatusCode = context.Response.StatusCode,
                Message = "Ocorreu um erro interno no servidor",
                Errors = new List<string> { exception.Message },
                StackTrace = exception.StackTrace,
                InnerException = exception.InnerException?.Message
            }
            : new ErrorResponse
            {
                StatusCode = context.Response.StatusCode,
                Message = "Ocorreu um erro interno no servidor",
                Errors = new List<string> { "Entre em contato com o suporte" }
            };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public string? StackTrace { get; set; }
    public string? InnerException { get; set; }
}