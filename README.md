# WebSite - ASP.NET Core Web API с PostgreSQL

Проект веб-приложения на ASP.NET Core 9.0 с использованием Clean Architecture, PostgreSQL, Entity Framework Core и Docker.

## Архитектура проекта

Проект следует принципам Clean Architecture и разделен на слои:

- **WebSite.Api** - Presentation Layer (API контроллеры, Swagger)
- **WebSite.Application** - Application Layer (MediatR handlers, бизнес-логика)
- **WebSite.Domain** - Domain Layer (сущности, интерфейсы)
- **WebSite.Infrastructure** - Infrastructure Layer (EF Core, репозитории, AutoMapper)

## Технологический стек

- .NET 9.0
- ASP.NET Core Web API
- PostgreSQL 16
- Entity Framework Core 9.0
- MediatR (CQRS pattern)
- AutoMapper
- Swagger/OpenAPI
- Docker & Docker Compose

## Требования

### Для локальной разработки:
- .NET SDK 9.0 или выше
- PostgreSQL 16 (или Docker)
- Visual Studio 2022 / VS Code / Rider

### Для Docker развертывания:
- Docker Desktop / Docker Engine
- Docker Compose

## Быстрый старт

### Вариант 1: Запуск с Docker Compose (рекомендуется)

1. **Создайте файлы с секретами:**
```bash
mkdir secrets
echo "your_username" > secrets/db_user.txt
echo "your_secure_password" > secrets/db_password.txt
```

2. **Запустите приложение:**
```bash
docker-compose up -d
```

3. **Приложение доступно по адресам:**
- API: http://localhost:5000
- HTTPS: https://localhost:5001
- PostgreSQL: localhost:5444

### Вариант 2: Локальная разработка

1. **Установите зависимости:**
```bash
dotnet restore
```

2. **Настройте подключение к базе данных:**

Создайте `WebSite.Api/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5444;Database=web_site;Username=your_user;Password=your_password"
  }
}
```

Или используйте User Secrets (рекомендуется):
```bash
cd WebSite.Api
dotnet user-secrets set "DB_USER" "your_username"
dotnet user-secrets set "DB_PASSWORD" "your_password"
```

3. **Запустите PostgreSQL:**
```bash
docker-compose up -d postgres
```

4. **Запустите приложение:**
```bash
cd WebSite.Api
dotnet run
```

5. **Откройте Swagger UI:**
```
https://localhost:5001/swagger
```

## Конфигурация

### Переменные окружения

Приложение поддерживает следующие переменные окружения:

| Переменная | Описание | По умолчанию |
|------------|----------|--------------|
| `DB_HOST` | Хост PostgreSQL | `postgres` |
| `DB_PORT` | Порт PostgreSQL | `5432` |
| `DB_NAME` | Имя базы данных | `web_site` |
| `DB_USER` | Имя пользователя БД | - |
| `DB_PASSWORD` | Пароль БД | - |
| `ASPNETCORE_ENVIRONMENT` | Окружение | `Production` |
| `ASPNETCORE_URLS` | URL для прослушивания | `http://+:8080` |

### Docker Secrets

В production окружении приложение автоматически читает учетные данные из Docker secrets:
- `/run/secrets/db_user`
- `/run/secrets/db_password`

## Развертывание в Production

### 1. Подготовка секретов

```bash
# Создайте безопасные пароли
mkdir secrets
echo "production_user" > secrets/db_user.txt
openssl rand -base64 32 > secrets/db_password.txt
chmod 600 secrets/*
```

### 2. Обновите CORS настройки

В `WebSite.Api/Program.cs` замените `yourdomain.com` на ваш реальный домен:

```csharp
policy.WithOrigins("https://yourdomain.com")
```

### 3. Запустите контейнеры

```bash
docker-compose up -d
```

### 4. Проверьте статус

```bash
docker-compose ps
docker-compose logs -f
```

## Миграции базы данных

### Создать новую миграцию:
```bash
cd WebSite.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../WebSite.Api
```

### Применить миграции:
```bash
dotnet ef database update --startup-project ../WebSite.Api
```

### Откатить миграцию:
```bash
dotnet ef database update PreviousMigrationName --startup-project ../WebSite.Api
```

## Сборка проекта

### Development сборка:
```bash
dotnet build
```

### Production сборка:
```bash
dotnet build --configuration Release
```

### Публикация:
```bash
dotnet publish WebSite.Api/WebSite.Api.csproj -c Release -o ./publish
```

## Тестирование

Запуск тестов (когда будут добавлены):
```bash
dotnet test
```

## Структура базы данных

### Таблицы:
- `Categories` - категории
- `CategoryImages` - изображения категорий

Подробнее см. в `WebSite.Domain/Entities/`

## API Endpoints

### Categories

- `GET /api/categories` - получить все категории
- `GET /api/categories/{id}` - получить категорию по ID
- `POST /api/categories` - создать категорию
- `PUT /api/categories/{id}` - обновить категорию
- `DELETE /api/categories/{id}` - удалить категорию

Полная документация API доступна в Swagger UI: `/swagger`

## Логирование

Логи приложения находятся в:
- Development: консоль
- Production: `./logs/` (при запуске через Docker)

## Мониторинг

### Проверка здоровья БД:
```bash
docker-compose exec postgres pg_isready -U $(cat secrets/db_user.txt)
```

### Просмотр логов:
```bash
docker-compose logs -f webapi
docker-compose logs -f postgres
```

## Безопасность

- Никогда не коммитьте файлы с секретами в git
- Используйте `.gitignore` для исключения конфиденциальных данных
- В production используйте HTTPS
- Регулярно обновляйте зависимости: `dotnet list package --outdated`
- Используйте Docker secrets для хранения паролей

## Устранение неполадок

### Порт уже занят:
Измените порты в `docker-compose.yml`

### Ошибка подключения к БД:
Проверьте, что PostgreSQL запущен и доступен:
```bash
docker-compose ps postgres
docker-compose logs postgres
```

### Приложение не запускается:
Проверьте логи:
```bash
docker-compose logs webapi
```

## Лицензия

Указать вашу лицензию

## Контакты

Указать контактную информацию для поддержки