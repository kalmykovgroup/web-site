using Microsoft.EntityFrameworkCore;
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
            builder.Services.AddControllers();

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
            });

            // �����������
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    if (builder.Environment.IsDevelopment())
                    {
                        policy.WithOrigins("http://localhost:5175")
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

            // Database initialization and seeding
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                if (app.Environment.IsDevelopment())
                {
                    // В Development пересоздаем БД каждый раз
                    logger.LogInformation("Development: Recreating database...");
                    dbContext.Database.EnsureDeleted();
                    dbContext.Database.EnsureCreated();
                    DatabaseSeeder.SeedCategories(dbContext);
                    logger.LogInformation("Development: Database recreated and seeded");
                }
                else
                {
                    // В Production создаем БД только если её нет
                    try
                    {
                        var canConnect = dbContext.Database.CanConnect();

                        if (canConnect)
                        {
                            // Проверяем есть ли таблицы
                            var tableExists = dbContext.Categories.Any();
                            logger.LogInformation("Production: Database tables exist, skipping initialization");
                        }
                        else
                        {
                            logger.LogInformation("Production: Database not found, creating...");
                            dbContext.Database.EnsureCreated();
                            DatabaseSeeder.SeedCategories(dbContext);
                            logger.LogInformation("Production: Database created and seeded");
                        }
                    }
                    catch (Exception ex) when (ex.Message.Contains("does not exist") || ex.Message.Contains("42P01"))
                    {
                        // Таблицы не существуют - создаем
                        logger.LogInformation("Production: Database tables not found, creating...");
                        dbContext.Database.EnsureCreated();
                        DatabaseSeeder.SeedCategories(dbContext);
                        logger.LogInformation("Production: Database created and seeded");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error during database initialization");
                        throw;
                    }
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

            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");

            // ����������� ����� ��� React ����������
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    // ����������� ����������� ������ (js, css, �����������)
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

            // SPA Fallback - ��������������� ���� ��-API �������� �� index.html
            app.MapFallbackToFile("index.html");

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