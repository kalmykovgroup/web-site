using Microsoft.EntityFrameworkCore;
using WebSite.Api.Middleware;
using WebSite.Application.Handlers;
using WebSite.Domain.Interfaces;
using WebSite.Infrastructure.Database;
using WebSite.Infrastructure.Database.Seed;
using WebSite.Infrastructure.Mapper;
using WebSite.Infrastructure.Repositories;
using WebSite.Infrastructure.Swagger;

namespace WebSite.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //  Sensitive Data Logging  Development
            builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Model.Validation", LogLevel.Error);

            // Add services to the container.
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Настройка для правильной сериализации enum в camelCase
                    options.JsonSerializerOptions.Converters.Add(
                        new System.Text.Json.Serialization.JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase));

                    // Настройка для camelCase свойств
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;

                    // Игнорировать null значения
                    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                });

            // Response Compression production
            if (!builder.Environment.IsDevelopment())
            {
                builder.Services.AddResponseCompression(options =>
                {
                    options.EnableForHttps = true;
                });
            }

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddCommonSwaggerServices();

            // DbContext � PostgreSQL
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

                // ���� ConnectionString �� ������, ������� �������� �� � ��������� ����������, ����� �� Docker Secrets
                if (string.IsNullOrEmpty(connectionString) || !connectionString.Contains("Password="))
                {
                    // ������� ������ �� ���������� ��������� ��� Docker Secrets
                    var dbHost = builder.Configuration["DB_HOST"] ?? "postgres";
                    var dbPort = builder.Configuration["DB_PORT"] ?? "5432";
                    var dbName = builder.Configuration["DB_NAME"] ?? "web_site";
                    var dbUser = builder.Configuration["DB_USER"] ?? ReadDockerSecret("db_user") ?? throw new InvalidOperationException("DB_USER not found");
                    var dbPassword = builder.Configuration["DB_PASSWORD"] ?? ReadDockerSecret("db_password") ?? throw new InvalidOperationException("DB_PASSWORD not found");

                    connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";
                }

                options.UseNpgsql(
                    connectionString,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly("WebSite.Infrastructure");
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorCodesToAdd: null);
                    });

                if (builder.Environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });

            // MediatR 
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(typeof(GetAllCategoriesHandler).Assembly);
            });

            // AutoMapper - ������������� ������ ��� Profile � ��������� ������ 
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<CategoryProfile>();
                cfg.AddProfile<CategoryImageProfile>();
                cfg.AddProfile<SpecialOfferProfile>();
            });

            // �����������
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ISpecialOffersRepository, SpecialOffersRepository>();

            // Получаем connection string для использования в Health Checks
            var healthCheckConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(healthCheckConnectionString) || !healthCheckConnectionString.Contains("Password="))
            {
                var dbHost = builder.Configuration["DB_HOST"] ?? "postgres";
                var dbPort = builder.Configuration["DB_PORT"] ?? "5432";
                var dbName = builder.Configuration["DB_NAME"] ?? "web_site";
                var dbUser = builder.Configuration["DB_USER"] ?? ReadDockerSecret("db_user") ?? throw new InvalidOperationException("DB_USER not found");
                var dbPassword = builder.Configuration["DB_PASSWORD"] ?? ReadDockerSecret("db_password") ?? throw new InvalidOperationException("DB_PASSWORD not found");

                healthCheckConnectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";
            }

            // Health Checks для мониторинга состояния БД
            builder.Services.AddHealthChecks()
                .AddDbContextCheck<AppDbContext>(
                    name: "database",
                    failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
                    tags: new[] { "db", "postgres" })
                .AddNpgSql(
                    healthCheckConnectionString,
                    name: "postgres",
                    failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
                    tags: new[] { "db", "postgres" });

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    if (builder.Environment.IsDevelopment())
                    {
                        policy.WithOrigins("http://localhost:5175", "https://localhost:5171", "https://localhost:5173")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    }
                    else
                    {
                        // Production CORS - читаем из переменной окружения
                        var corsOrigins = builder.Configuration["CORS_ORIGINS"];

                        if (!string.IsNullOrEmpty(corsOrigins))
                        {
                            var origins = corsOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                    .Select(o => o.Trim())
                                                    .ToArray();

                            policy.WithOrigins(origins)
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                        }
                        else
                        {
                            // Fallback если CORS_ORIGINS не задан
                            policy.WithOrigins("https://volnaya-28.ru")
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                        }
                    }
                });
            });

            var app = builder.Build();

            // Database initialization and seeding with retry policy
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                // Реализуем retry логику с экспоненциальной задержкой
                try
                {
                    const int maxRetries = 5;
                    int retryCount = 0;
                    bool success = false;

                    while (retryCount < maxRetries && !success)
                    {
                        try
                        {
                            if (app.Environment.IsDevelopment())
                            {
                                // В Development пересоздаем БД каждый раз
                                logger.LogInformation("Development: Recreating database...");
                                dbContext.Database.EnsureDeleted();
                                dbContext.Database.EnsureCreated();
                                DatabaseSeeder.SeedCategories(dbContext);
                                DatabaseSeeder.SeedSpecialOffers(dbContext);
                                logger.LogInformation("Development: Database recreated and seeded");
                            }
                            else
                            {
                                // В Production создаем БД только если её нет
                                var canConnect = dbContext.Database.CanConnect();

                                if (canConnect)
                                {
                                    // Проверяем есть ли таблицы
                                    try
                                    {
                                        var tableExists = dbContext.Categories.Any();
                                        logger.LogInformation("Production: Database tables exist, skipping initialization");
                                    }
                                    catch (Exception ex) when (ex.Message.Contains("does not exist") || ex.Message.Contains("42P01"))
                                    {
                                        // Таблицы не существуют - создаем
                                        logger.LogInformation("Production: Database tables not found, creating...");
                                        dbContext.Database.EnsureCreated();
                                        DatabaseSeeder.SeedCategories(dbContext);
                                        DatabaseSeeder.SeedSpecialOffers(dbContext);
                                        logger.LogInformation("Production: Database created and seeded");
                                    }
                                }
                                else
                                {
                                    logger.LogInformation("Production: Database not found, creating...");
                                    dbContext.Database.EnsureCreated();
                                    DatabaseSeeder.SeedCategories(dbContext);
                                    DatabaseSeeder.SeedSpecialOffers(dbContext);
                                    logger.LogInformation("Production: Database created and seeded");
                                }
                            }

                            success = true;
                        }
                        catch (Exception ex) when (
                            ex is Npgsql.NpgsqlException ||
                            ex is System.Net.Sockets.SocketException ||
                            ex is TimeoutException)
                        {
                            retryCount++;
                            if (retryCount < maxRetries)
                            {
                                var delay = TimeSpan.FromSeconds(Math.Pow(2, retryCount));
                                logger.LogWarning(
                                    ex,
                                    "Database connection attempt {RetryCount} of {MaxRetries} failed. Waiting {Delay} seconds before next retry...",
                                    retryCount,
                                    maxRetries,
                                    delay.TotalSeconds);

                                Thread.Sleep(delay);
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // После всех попыток БД все еще недоступна - логируем и продолжаем работу
                    logger.LogCritical(
                        ex,
                        "CRITICAL: Unable to connect to database after multiple retries. " +
                        "Application will start but database operations will fail. " +
                        "Please check database connectivity and configuration.");

                    // Не бросаем исключение - позволяем приложению запуститься
                    // Health checks покажут, что БД недоступна
                }
            }

            // Swagger ������ ��� Development
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Charts API v1"));
            }

            // Response Compression ������ ��� production
            if (!app.Environment.IsDevelopment())
            {
                app.UseResponseCompression();
            }

            // Content Security Policy
            app.Use(async (context, next) =>
            {
                // Разные CSP для dev и production
                var cspPolicy = app.Environment.IsDevelopment()
                    ? "default-src 'self'; " +
                      "script-src 'self' 'unsafe-eval' 'unsafe-inline' https://yandex.ru https://api-maps.yandex.ru https://mc.yandex.ru https://mc.yandex.com; " +
                      "style-src 'self' 'unsafe-inline' https:; " +
                      "img-src 'self' data: https: https://mc.yandex.ru https://mc.yandex.com; " +
                      "font-src 'self' data: https:; " +
                      "connect-src 'self' https: ws: wss: http://localhost:5175 https://localhost:5171 ws://localhost:5175 wss://mc.yandex.com https://mc.yandex.ru https://mc.yandex.com; " +
                      "frame-src https://yandex.ru https://api-maps.yandex.ru https://mc.yandex.com;"
                    : "default-src 'self'; " +
                      "script-src 'self' 'unsafe-inline' https://yandex.ru https://api-maps.yandex.ru https://mc.yandex.ru https://mc.yandex.com; " +
                      "style-src 'self' 'unsafe-inline' https:; " +
                      "img-src 'self' data: https: https://mc.yandex.ru https://mc.yandex.com; " +
                      "font-src 'self' data: https:; " +
                      "connect-src 'self' https: wss://mc.yandex.com https://mc.yandex.ru https://mc.yandex.com; " +
                      "frame-src https://yandex.ru https://api-maps.yandex.ru https://mc.yandex.com; " +
                      "frame-ancestors 'none'; " +
                      "base-uri 'self'; " +
                      "form-action 'self'";

                context.Response.Headers.Append("Content-Security-Policy", cspPolicy);

                // Additional security headers
                context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Append("X-Frame-Options", "DENY");
                context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
                context.Response.Headers.Append("Permissions-Policy", "geolocation=(), microphone=(), camera=()");

                await next();
            });

            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");

            // Глобальная обработка ошибок БД
            app.UseDatabaseExceptionHandling();

            // Статические файлы для React приложения
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            // Добавляем явные MIME-типы для всех типов файлов
            provider.Mappings[".html"] = "text/html; charset=utf-8";
            provider.Mappings[".css"] = "text/css; charset=utf-8";
            provider.Mappings[".js"] = "application/javascript; charset=utf-8";
            provider.Mappings[".json"] = "application/json; charset=utf-8";
            provider.Mappings[".xml"] = "application/xml; charset=utf-8";
            provider.Mappings[".txt"] = "text/plain; charset=utf-8";
            provider.Mappings[".svg"] = "image/svg+xml";
            provider.Mappings[".png"] = "image/png";
            provider.Mappings[".jpg"] = "image/jpeg";
            provider.Mappings[".jpeg"] = "image/jpeg";
            provider.Mappings[".gif"] = "image/gif";
            provider.Mappings[".ico"] = "image/x-icon";
            provider.Mappings[".woff"] = "font/woff";
            provider.Mappings[".woff2"] = "font/woff2";
            provider.Mappings[".ttf"] = "font/ttf";
            provider.Mappings[".eot"] = "application/vnd.ms-fontobject";
            provider.Mappings[".webmanifest"] = "application/manifest+json";

            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider,
                OnPrepareResponse = ctx =>
                {
                    // Долгосрочное кеширование ресурсов (js, css, изображения)
                    if (ctx.File.Name.EndsWith(".js") ||
                        ctx.File.Name.EndsWith(".css") ||
                        ctx.File.Name.EndsWith(".woff") ||
                        ctx.File.Name.EndsWith(".woff2") ||
                        ctx.File.Name.EndsWith(".png") ||
                        ctx.File.Name.EndsWith(".jpg") ||
                        ctx.File.Name.EndsWith(".svg"))
                    {
                        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=31536000,immutable");
                    }
                }
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Health Check endpoints
            app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var result = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        timestamp = DateTime.UtcNow,
                        duration = report.TotalDuration,
                        checks = report.Entries.Select(e => new
                        {
                            name = e.Key,
                            status = e.Value.Status.ToString(),
                            description = e.Value.Description,
                            duration = e.Value.Duration,
                            exception = e.Value.Exception?.Message,
                            data = e.Value.Data
                        })
                    });
                    await context.Response.WriteAsync(result);
                }
            });

            // Простой liveness endpoint для быстрой проверки, что приложение работает
            app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = _ => false // Не проверяет зависимости, только то что приложение запущено
            });

            // Readiness endpoint - проверяет готовность к обработке запросов (включая БД)
            app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("db")
            });

            // SPA Fallback - перенаправляем только не-статические файлы на index.html
            // Исключаем важные статические файлы (sitemap, robots, manifest, sw)
            app.MapFallback(async context =>
            {
                var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";

                // Список файлов которые НЕ должны попадать под SPA fallback
                var staticFiles = new[]
                {
                    "/sitemap.xml",
                    "/robots.txt",
                    "/manifest.json",
                    "/sw.js",
                    "/registersw.js",
                    "/.htaccess",
                    "/_redirects"
                };

                // Если запрос к одному из статических файлов - пропускаем fallback
                if (staticFiles.Any(file => path.Equals(file, StringComparison.OrdinalIgnoreCase)))
                {
                    context.Response.StatusCode = 404;
                    return;
                }

                // Если запрос к workbox файлам - пропускаем fallback
                if (path.StartsWith("/workbox-") && path.EndsWith(".js"))
                {
                    context.Response.StatusCode = 404;
                    return;
                }

                // Для всех остальных запросов - отдаем index.html (SPA fallback)
                context.Request.Path = "/index.html";
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.SendFileAsync(Path.Combine(context.RequestServices.GetRequiredService<IWebHostEnvironment>().WebRootPath, "index.html"));
            });

            app.Run();
        }

        /// <summary>
        /// ������ Docker secret �� ����� /run/secrets/
        /// ������������ ��� ��������� credentials �� Docker Secrets � Production
        /// </summary>
        /// <param name="secretName">��� secret (��������, "db_user" ��� "db_password")</param>
        /// <returns>���������� secret ��� null ���� ���� �� ������</returns>
        private static string? ReadDockerSecret(string secretName)
        {
            var secretPath = $"/run/secrets/{secretName}";

            if (File.Exists(secretPath))
            {
                return File.ReadAllText(secretPath).Trim();
            }

            return null;
        }
    }
}