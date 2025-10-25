# Базовый образ для runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем .csproj файлы всех проектов
COPY ["WebSite.Api/WebSite.Api.csproj", "WebSite.Api/"]
COPY ["WebSite.Infrastructure/WebSite.Infrastructure.csproj", "WebSite.Infrastructure/"]
COPY ["WebSite.Application/WebSite.Application.csproj", "WebSite.Application/"]
COPY ["WebSite.Domain/WebSite.Domain.csproj", "WebSite.Domain/"]

# Восстанавливаем зависимости
RUN dotnet restore "WebSite.Api/WebSite.Api.csproj"

# Копируем весь исходный код
COPY . .

# Собираем проект
WORKDIR "/src/WebSite.Api"
RUN dotnet build "WebSite.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этап публикации
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebSite.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# Финальный образ
FROM base AS final
WORKDIR /app

# Копируем опубликованное приложение
COPY --from=publish /app/publish .

# Копируем wwwroot (React build) если существует
COPY --from=build /src/WebSite.Api/wwwroot ./wwwroot

# Создаем пользователя для запуска приложения (безопасность)
RUN addgroup --system --gid 1000 appgroup \
    && adduser --system --uid 1000 --ingroup appgroup --shell /bin/sh appuser

# Устанавливаем владельца файлов
RUN chown -R appuser:appgroup /app

# Переключаемся на непривилегированного пользователя
USER appuser

# Точка входа
ENTRYPOINT ["dotnet", "WebSite.Api.dll"]