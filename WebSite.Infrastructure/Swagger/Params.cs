using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebSite.Infrastructure.Swagger
{
    public static class SwaggerParams
    {
        public static void Add(OpenApiOperation operation, OperationFilterContext context, string route, string key, string value)
        {
            var routeTemplate = context.ApiDescription.ActionDescriptor?.AttributeRouteInfo?.Template ?? string.Empty;

            if (routeTemplate != route) return; 


            operation.Parameters ??= new List<OpenApiParameter>();

          

            foreach (var param in operation.Parameters)
            {

                if (param.Name.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    param.Schema.Example = new OpenApiString(value);
                    param.Example = new OpenApiString(value);
                }
            }
             

        }

        public static void AddHeader(OpenApiOperation operation, OperationFilterContext context, string route, string key, string value)
        {
            var routeTemplate = context.ApiDescription.ActionDescriptor?.AttributeRouteInfo?.Template ?? string.Empty;

            if (routeTemplate != route) return;

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = key,
                In = ParameterLocation.Header,
                Required = true,                  // сделайте false, если не обязателен
                Description = "Ключ выбора базы",
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = new OpenApiString(value) // заполнит поле по умолчанию
                },
                Example = new OpenApiString(value)     // и покажет пример
            });
        }
        public static void AddHeaders(OpenApiOperation operation, OperationFilterContext context, string route, Dictionary<string, string> headers)
        {
            var routeTemplate = context.ApiDescription.ActionDescriptor?.AttributeRouteInfo?.Template ?? string.Empty;

            if (routeTemplate != route) return;

            foreach (var param in headers)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = param.Key,
                    In = ParameterLocation.Header,
                    Required = true,                  // сделайте false, если не обязателен
                    Description = "Ключ выбора базы",
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Default = new OpenApiString(param.Value) // заполнит поле по умолчанию
                    },
                    Example = new OpenApiString(param.Value)     // и покажет пример
                });
            }
        }

        public static void Add(OpenApiOperation operation, OperationFilterContext context, string route, string key, int value)
        {
            var routeTemplate = context.ApiDescription.ActionDescriptor?.AttributeRouteInfo?.Template ?? string.Empty;

            if (routeTemplate != route) return;

            if (operation.Parameters == null) return;

            foreach (var param in operation.Parameters)
            {

                if (param.Name.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    param.Schema.Example = new OpenApiInteger(value);
                    param.Example = new OpenApiInteger(value);
                }
            }

        }

        public static void Add<T>(OpenApiOperation operation, OperationFilterContext context, string route, T body) where T : class
        {
            string? routeTemplate = context.ApiDescription.ActionDescriptor?.AttributeRouteInfo?.Template ?? string.Empty;

            if (routeTemplate != route)
                return;

            if (operation.RequestBody == null || !operation.RequestBody.Content.TryGetValue("application/json", out var content))
                return;

            if (content.Schema.Reference != null)
            {
                var schemaName = content.Schema.Reference.Id;
                if (context.SchemaRepository.Schemas.TryGetValue(schemaName, out var schema))
                {
                    schema.Example = OpenApiExampleHelper.ToOpenApiAny(body);
                }
            }

        }
    }
}
