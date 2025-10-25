using Microsoft.OpenApi.Any;

namespace WebSite.Infrastructure.Swagger
{
    public static class OpenApiExampleHelper
    {
        public static IOpenApiAny ToOpenApiAny(object obj)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(obj);
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            return ParseJsonElement(doc.RootElement);
        }

        private static IOpenApiAny ParseJsonElement(System.Text.Json.JsonElement element)
        {
            switch (element.ValueKind)
            {
                case System.Text.Json.JsonValueKind.Object:
                    var openApiObject = new OpenApiObject();
                    foreach (var property in element.EnumerateObject())
                    {
                        openApiObject.Add(property.Name, ParseJsonElement(property.Value));
                    }
                    return openApiObject;

                case System.Text.Json.JsonValueKind.Array:
                    var openApiArray = new OpenApiArray();
                    foreach (var item in element.EnumerateArray())
                    {
                        openApiArray.Add(ParseJsonElement(item));
                    }
                    return openApiArray;

                case System.Text.Json.JsonValueKind.String:
                    return new OpenApiString(element.GetString() ?? string.Empty);

                case System.Text.Json.JsonValueKind.Number:
                    if (element.TryGetInt32(out var intVal))
                        return new OpenApiInteger(intVal);
                    if (element.TryGetInt64(out var longVal))
                        return new OpenApiLong(longVal);
                    if (element.TryGetDouble(out var doubleVal))
                        return new OpenApiDouble(doubleVal);
                    return new OpenApiString(element.GetRawText());

                case System.Text.Json.JsonValueKind.True:
                    return new OpenApiBoolean(true);
                case System.Text.Json.JsonValueKind.False:
                    return new OpenApiBoolean(false);
                case System.Text.Json.JsonValueKind.Null:
                default:
                    return new OpenApiNull();
            }
        }
    }
}
