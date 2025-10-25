# Быстрый старт

## Локальная разработка (5 минут)

```bash
# 1. Клонируйте репозиторий
git clone <repository-url>
cd WebSite

# 2. Создайте секреты для БД
mkdir secrets
echo "dev_user" > secrets/db_user.txt
echo "dev_password" > secrets/db_password.txt

# 3. Запустите все сервисы
docker-compose up -d

# 4. Откройте в браузере
# API: http://localhost
# Swagger: http://localhost/swagger
# PostgreSQL: localhost:5444
```

## Production развертывание (10 минут)

```bash
# 1. Клонируйте на сервер
git clone <repository-url>
cd WebSite

# 2. Настройте окружение
cp .env.example .env
nano .env
# Измените DOMAIN=yourdomain.com

# 3. Создайте безопасные секреты
mkdir secrets
echo "production_user" > secrets/db_user.txt
openssl rand -base64 32 > secrets/db_password.txt
chmod 600 secrets/*

# 4. Убедитесь что .env настроен с вашим доменом
nano .env
# DOMAIN=yourdomain.com
# EMAIL=your@email.com

# 5. Запустите сервисы
docker compose up -d

# SSL настроится автоматически!

# 6. Готово!
# Ваше приложение доступно: https://yourdomain.com
```

## Проверка

```bash
# Статус всех контейнеров
docker-compose ps

# Логи приложения
docker-compose logs -f webapi

# Проверка БД
docker-compose exec postgres pg_isready -U $(cat secrets/db_user.txt)

# Проверка SSL
docker-compose exec nginx nginx -t
```

## Что дальше?

- [Полная документация](README.md)
- [Настройка SSL](SSL-SETUP.md)
- [API документация](http://localhost/swagger)

## Проблемы?

```bash
# Очистить все и начать заново
docker-compose down -v
rm -rf certbot/conf/*
docker-compose up -d
```
