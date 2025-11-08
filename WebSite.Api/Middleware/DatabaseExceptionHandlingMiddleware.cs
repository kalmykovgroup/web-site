using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace WebSite.Api.Middleware
{
    /// <summary>
    /// Middleware для глобальной обработки ошибок базы данных
    /// Обеспечивает graceful degradation при недоступности БД
    /// </summary>
    public class DatabaseExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DatabaseExceptionHandlingMiddleware> _logger;

        public DatabaseExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<DatabaseExceptionHandlingMiddleware> logger)
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
            catch (NpgsqlException ex)
            {
                await HandleDatabaseExceptionAsync(context, ex);
            }
            catch (DbUpdateException ex)
            {
                await HandleDatabaseExceptionAsync(context, ex);
            }
            catch (TimeoutException ex) when (ex.Message.Contains("database") || ex.Message.Contains("connection"))
            {
                await HandleDatabaseExceptionAsync(context, ex);
            }
        }

        private async Task HandleDatabaseExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(
                exception,
                "Database operation failed for {Method} {Path}. " +
                "This may indicate database connectivity issues.",
                context.Request.Method,
                context.Request.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;

            object response;

            // В Development режиме добавляем детали ошибки
            if (context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                response = new
                {
                    error = "Service Temporarily Unavailable",
                    message = "The database is currently unavailable. Please try again later.",
                    timestamp = DateTime.UtcNow,
                    path = context.Request.Path.ToString(),
                    details = exception.Message,
                    exceptionType = exception.GetType().Name
                };
            }
            else
            {
                response = new
                {
                    error = "Service Temporarily Unavailable",
                    message = "The database is currently unavailable. Please try again later.",
                    timestamp = DateTime.UtcNow,
                    path = context.Request.Path.ToString()
                };
            }

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }

    /// <summary>
    /// Extension methods для упрощения регистрации middleware
    /// </summary>
    public static class DatabaseExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseDatabaseExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DatabaseExceptionHandlingMiddleware>();
        }
    }
}
