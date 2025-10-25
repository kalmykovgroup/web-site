using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace WebSite.Infrastructure.Swagger
{
    public static class SwaggerGenOptionsExtensions
    {
        /// <summary>
        /// Регистрирует все IOperationFilter из указанных сборок.
        /// Если assemblies не переданы — сканирует все загруженные сборки по вашим префиксам.
        /// </summary>
        public static void AddAllOperationFiltersFrom(this SwaggerGenOptions options, params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length == 0)
            {
                assemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic)
                    .Where(a =>
                    {
                        var n = a.GetName().Name ?? "";
                        return n.StartsWith("RemoteWeb", StringComparison.OrdinalIgnoreCase)
                            || n.StartsWith("Api.", StringComparison.OrdinalIgnoreCase)
                            || n.StartsWith("ApiShared", StringComparison.OrdinalIgnoreCase);
                    })
                    .ToArray();
            }

            var types = assemblies
                .SelectMany(a =>
                {
                    try { return a.GetExportedTypes(); }
                    catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t != null)!; }
                })
                .Where(t => t is { IsClass: true, IsAbstract: false } &&
                            typeof(IOperationFilter).IsAssignableFrom(t!))
                .Distinct()
                .OrderBy(t => t!.FullName);

            foreach (var t in types!)
            {
                if (options.OperationFilterDescriptors.Any(d => d.Type == t)) continue;

                options.OperationFilterDescriptors.Add(new FilterDescriptor
                {
                    Type = t!,
                    Arguments = Array.Empty<object>() // зависимости конструктора подтянет DI
                });
            }
        }

        // (Опционально) то же для других видов фильтров
        public static void AddAllDocumentFiltersFrom(this SwaggerGenOptions options, params Assembly[] assemblies)
            => AddAllFiltersFrom<IDocumentFilter>(options.DocumentFilterDescriptors, assemblies);

        public static void AddAllSchemaFiltersFrom(this SwaggerGenOptions options, params Assembly[] assemblies)
            => AddAllFiltersFrom<ISchemaFilter>(options.SchemaFilterDescriptors, assemblies);

        public static void AddAllParameterFiltersFrom(this SwaggerGenOptions options, params Assembly[] assemblies)
            => AddAllFiltersFrom<IParameterFilter>(options.ParameterFilterDescriptors, assemblies);

        private static void AddAllFiltersFrom<TFilter>(IList<FilterDescriptor> target, Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length == 0)
            {
                assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).ToArray();
            }

            var types = assemblies
                .SelectMany(a =>
                {
                    try { return a.GetExportedTypes(); }
                    catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t != null)!; }
                })
                .Where(t => t is { IsClass: true, IsAbstract: false } &&
                            typeof(TFilter).IsAssignableFrom(t!))
                .Distinct()
                .OrderBy(t => t!.FullName);

            foreach (var t in types!)
            {
                if (target.Any(d => d.Type == t)) continue;
                target.Add(new FilterDescriptor { Type = t!, Arguments = Array.Empty<object>() });
            }
        }
    }
}
