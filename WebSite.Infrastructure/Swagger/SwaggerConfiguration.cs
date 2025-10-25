using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace WebSite.Infrastructure.Swagger
{
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "RemoteWeb API",
                    Version = "v1",
                    Description = "API для работы с RemoteWeb",
                    Contact = new OpenApiContact
                    {
                        Name = "Ваша компания",
                        Email = "support@example.com"
                    }
                });

                // Добавим поддержку авторизации по JWT (чтобы в Swagger можно было вставить токен)
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });


                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });


 

                // ВАЖНО: добавляем OperationFilter для корректной работы с [Authorize]
                // options.OperationFilter<AuthorizeCheckOperationFilter>();

                // Примеры (Examples)
                options.ExampleFilters();
            });

            return services;
        }

    }
}
