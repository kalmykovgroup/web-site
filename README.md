# WebSite - ASP.NET Core Web API с PostgreSQL

Проект веб-приложения на ASP.NET Core 9.0 с использованием Clean Architecture, PostgreSQL, Entity Framework Core и Docker.

---

## 🚀 Быстрый запуск

**Запуск в два шага:**
```bash
# 1. Создайте секреты для БД
mkdir -p secrets
echo "your_username" > secrets/db_user.txt
echo "your_secure_password" > secrets/db_password.txt
chmod 600 secrets/*

# 2. Запустите проект
docker compose up -d
```

**Для развертывания на сервере смотрите:** [START-HERE.md](START-HERE.md)

---

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
- Nginx (reverse proxy)
- Let's Encrypt SSL (автоматическое управление)

## Требования

### Для локальной разработки:
- .NET SDK 9.0 или выше
- PostgreSQL 16 (или Docker)
- Visual Studio 2022 / VS Code / Rider

### Для Docker развертывания:
- Docker Desktop / Docker Engine
- Docker Compose

## 🚀 Production развертывание

**Домен**: volnaya-28.ru | **Email**: admin@kalmykov-group.ru

Проект полностью готов к production развертыванию с автоматической настройкой SSL.

```bash
# Запуск (все настроено в .env)
docker compose up -d
```

SSL сертификат получается автоматически при первом запуске.

📚 **Подробная инструкция**: [START-HERE.md](START-HERE.md) | [DEPLOY.md](DEPLOY.md) | [PRODUCTION-READY.md](PRODUCTION-READY.md)

---

## Быстрый старт

### Вариант 1: Запуск с Docker Compose (рекомендуется)

1. **Убедитесь что .env файл настроен:**
```bash
cat .env
# Проверьте что DOMAIN, EMAIL, DB_USER и DB_PASSWORD заданы
```

2. **Запустите приложение:**
```bash
docker compose up -d
```

3. **Приложение доступно по адресам:**
- HTTP: http://localhost (или ваш домен)
- HTTPS: https://localhost (SSL настраивается автоматически)
- PostgreSQL: localhost:5444

**Примечание:** SSL сертификат получается автоматически если домен настроен корректно. Для подробностей см. [Настройка SSL](#настройка-ssl-сертификатов)

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
| `DB_USER` | Имя пользователя БД | - (из secrets) |
| `DB_PASSWORD` | Пароль БД | - (из secrets) |
| `ASPNETCORE_ENVIRONMENT` | Окружение | `Production` |
| `ASPNETCORE_URLS` | URL для прослушивания | `http://+:8080` |
| `DOMAIN` | Домен для SSL | `localhost` |
| `EMAIL` | Email для Let's Encrypt | - |
| `CORS_ORIGINS` | Разрешенные CORS origins (через запятую) | - |

### Конфигурация безопасности

**Docker Secrets (для production):**

Приложение использует Docker secrets для безопасного хранения учетных данных БД:
- `/run/secrets/db_user` - имя пользователя БД
- `/run/secrets/db_password` - пароль БД

Создайте файлы с секретами:
```bash
mkdir -p secrets
echo "your_username" > secrets/db_user.txt
echo "your_secure_password" > secrets/db_password.txt
chmod 600 secrets/*
```

**Переменные окружения из .env:**

Другие настройки хранятся в `.env`:
- `DOMAIN` - домен для SSL сертификата
- `EMAIL` - email для уведомлений Let's Encrypt
- `DB_HOST`, `DB_PORT`, `DB_NAME` - параметры подключения к БД
- `CORS_ORIGINS` - разрешенные CORS origins через запятую (например: `https://yourdomain.com,https://www.yourdomain.com`)

**Важно:** Файл `.env` не содержит паролей, они в `secrets/` директории

## Развертывание в Production

### 1. Создайте секреты БД

```bash
# Создайте безопасные учетные данные
mkdir -p secrets
echo "production_user" > secrets/db_user.txt
openssl rand -base64 32 > secrets/db_password.txt
chmod 600 secrets/*
```

### 2. Настройте .env файл

```bash
# Отредактируйте .env с вашими параметрами
nano .env
```

Убедитесь что заданы:
- `DOMAIN` - ваш домен
- `EMAIL` - email для SSL сертификата
- `CORS_ORIGINS` - разрешенные origins для CORS

Пример `.env`:
```env
DOMAIN=yourdomain.com
EMAIL=admin@yourdomain.com
CORS_ORIGINS=https://yourdomain.com,https://www.yourdomain.com,https://app.yourdomain.com
```

**Примечание:** `DB_USER` и `DB_PASSWORD` не указываются в `.env`, они в `secrets/` директории

### 3. Запустите контейнеры

```bash
docker compose up -d
```

### 4. Проверьте статус

```bash
docker compose ps
docker compose logs -f
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
docker-compose logs -f nginx
```

## Настройка SSL сертификатов

Проект включает **умную систему управления SSL** с автоматической проверкой валидности.

### Ключевые особенности

- ✅ **Умная проверка**: Сертификат запрашивается только если отсутствует или невалиден
- ✅ **Защита от повторных запросов**: `docker-compose build` не вызывает повторный выпуск
- ✅ **Автоматическое обновление**: Certbot обновляет сертификаты каждые 12 часов
- ✅ **Graceful fallback**: Приложение работает без SSL до его настройки

### Быстрая настройка для production

1. **Настройте DNS**: A-запись домена должна указывать на IP сервера

2. **Убедитесь что .env настроен**:
```bash
cat .env
# Проверьте DOMAIN и EMAIL
```

3. **Запустите приложение**:
```bash
docker compose up -d
```

Готово! SSL сертификат получится автоматически, и ваше приложение будет доступно по https://yourdomain.com

**Примечание:** Nginx автоматически:
- Проверит наличие SSL сертификата
- Если сертификата нет или он невалиден - получит новый от Let's Encrypt
- Настроит HTTPS конфигурацию

### Как работает защита

1. **При запуске контейнера**: Nginx проверяет наличие сертификата и его валидность (> 30 дней)
2. **Автоматическое получение**: Если сертификата нет или он невалиден, nginx получает новый через Let's Encrypt
3. **Автоматическое обновление**: Certbot обновляет только истекающие сертификаты каждые 12 часов

### Проверка статуса SSL

```bash
# Проверить наличие сертификата
docker-compose exec nginx ls -la /etc/letsencrypt/live/

# Проверить срок действия
docker-compose exec nginx openssl x509 -enddate -noout -in /etc/letsencrypt/live/yourdomain.com/fullchain.pem

# Логи certbot
docker-compose logs certbot
```

**Подробная документация**: См. [SSL-SETUP.md](SSL-SETUP.md)

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