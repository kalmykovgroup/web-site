# Инструкции по обновлению на сервере

## Что было сделано

### 1. Автоматическое создание БД при первом запуске
- ✅ БД создается автоматически при первом запуске в Production
- ✅ Таблицы создаются через `EnsureCreated()`
- ✅ Данные заполняются автоматически (seed data)
- ✅ При повторном запуске ничего не пересоздается

### 2. Безопасное хранение credentials
- ✅ DB_USER и DB_PASSWORD теперь в `secrets/` директории
- ✅ Не хранятся в `.env` файле
- ✅ Монтируются через Docker secrets

### 3. CORS конфигурация через .env
- ✅ CORS origins настраиваются через переменную `CORS_ORIGINS`
- ✅ Поддержка нескольких доменов через запятую

### 4. Автоматическое получение SSL
- ✅ Nginx автоматически получает SSL сертификат при первом запуске
- ✅ Проверяет валидность сертификата (> 30 дней)
- ✅ Обновляет если истекает

## Обновление на сервере

```bash
# 1. Загрузите последние изменения
cd /root/web-site
git pull

# 2. Создайте secrets если еще не создали
mkdir -p secrets
echo "kalmykov" > secrets/db_user.txt
echo "xK9mN2pQ7rL4wZ8vB6hT3jF5nY1dC0sA9eR7uX4kM2gH6tP8bV3qW" > secrets/db_password.txt
chmod 600 secrets/*

# 3. Проверьте .env файл
cat .env
# Должно быть:
# DOMAIN=volnaya-28.ru
# EMAIL=admin@kalmykov-group.ru
# CORS_ORIGINS=https://volnaya-28.ru,https://www.volnaya-28.ru

# 4. Остановите контейнеры
docker compose down

# 5. Удалите старый volume PostgreSQL (если пароль изменился)
docker volume rm web-site_postgres_data

# 6. Пересоберите и запустите
docker compose up -d --build

# 7. Следите за логами
docker logs -f website_api
```

## Что произойдет при запуске

1. **PostgreSQL запустится** с новым паролем из `secrets/db_password.txt`
2. **Web API подключится к БД** используя credentials из secrets
3. **При первом запуске:**
   - Проверит существование таблиц
   - Если таблиц нет - создаст их автоматически
   - Заполнит начальными данными (13 категорий)
   - Выведет в логи: `Production: Database created and seeded`
4. **При повторном запуске:**
   - Проверит что таблицы существуют
   - Ничего не пересоздаст
   - Выведет в логи: `Production: Database tables exist, skipping initialization`
5. **Nginx:**
   - Попытается получить SSL сертификат для volnaya-28.ru
   - Если DNS настроен - получит сертификат автоматически
   - Если нет - запустится в HTTP режиме

## Проверка работоспособности

```bash
# Проверка статуса контейнеров
docker compose ps

# Логи Web API (должно быть "Database created and seeded")
docker logs website_api | grep -i database

# Логи Nginx (попытка получения SSL)
docker logs website_nginx | grep -i ssl

# Проверка подключения к БД
docker compose exec postgres psql -U kalmykov -d web_site -c "SELECT COUNT(*) FROM \"Categories\";"
# Должно вернуть: 13

# Проверка API
curl http://volnaya-28.ru/api/categories
# Должно вернуть список категорий в JSON

# Проверка SSL (если получен)
curl -I https://volnaya-28.ru
```

## Устранение проблем

### БД не создается

```bash
# Проверьте логи API
docker logs website_api

# Проверьте что secrets доступны
docker compose exec website_api cat /run/secrets/db_user
docker compose exec website_api cat /run/secrets/db_password

# Проверьте подключение к PostgreSQL
docker compose exec postgres psql -U kalmykov -d web_site -c "\dt"
```

### SSL не получается

```bash
# Проверьте DNS
nslookup volnaya-28.ru
# Должен вернуть IP сервера

# Проверьте доступность порта 80
curl http://volnaya-28.ru/.well-known/acme-challenge/test

# Логи certbot
docker logs website_nginx | grep certbot
```

### CORS ошибки

```bash
# Проверьте что CORS_ORIGINS передается
docker compose exec website_api printenv | grep CORS

# Если не передается - проверьте .env
cat .env | grep CORS
```

## Откат изменений

Если что-то пошло не так:

```bash
# Вернитесь к предыдущей версии
git checkout HEAD~1

# Пересоберите
docker compose down
docker compose up -d --build
```

## Дополнительные команды

```bash
# Пересоздать БД (удалить все данные)
docker compose down
docker volume rm web-site_postgres_data
docker compose up -d

# Перезапустить только Web API
docker compose restart website_api

# Перезапустить только Nginx
docker compose restart website_nginx

# Посмотреть логи всех контейнеров
docker compose logs -f
```
