using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebSite.Infrastructure.Swagger.Filters
{ 
    public class TestSwaggerFilter : IOperationFilter
    {
        private static readonly string Route = "";

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //Body
            SwaggerParams.Add(operation, context, Route,  "");

            //Get
            SwaggerParams.Add(operation, context, Route, "id", "");

        }
    }
}
