using Microsoft.Extensions.DependencyInjection;

namespace WebSite.Infrastructure.Swagger
{
    public static class BuilderSwaggerExtensions
    {
        public static IServiceCollection AddCommonSwaggerServices(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                // добавит все IOperationFilter из ваших сборок
                c.AddAllOperationFiltersFrom(
                    typeof(BuilderSwaggerExtensions).Assembly  // текущая сборка
                                                               // , typeof(SomeKnownFilter).Assembly      // при желании — добавить ещё
                );

                // при необходимости — аналогично:
                // c.AddAllDocumentFiltersFrom();
                // c.AddAllSchemaFiltersFrom();
                // c.AddAllParameterFiltersFrom();
            });

            return services;
        }
    }

}
