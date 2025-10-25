# ✅ Проект готов к Production развертыванию

## Домен и контакты

- **Домен**: volnaya-28.ru
- **Email**: admin@kalmykov-group.ru

## Быстрый запуск

```bash
# 1. Убедитесь что .env настроен
cat .env
# Проверьте что DOMAIN, EMAIL, DB_USER и DB_PASSWORD заданы

# 2. Запустите
docker compose up -d

# 3. Готово! Сайт доступен на https://volnaya-28.ru
# SSL настроится автоматически
```

## Что настроено

### ✅ Конфигурация (.env)

```env
DOMAIN=volnaya-28.ru
EMAIL=admin@kalmykov-group.ru
```

Файл `.env` уже в репозитории с правильными значениями!

### ✅ Умное управление SSL

- Сертификат запрашивается **только при необходимости**
- `docker-compose build` **НЕ** запрашивает сертификат
- Автоматическое обновление каждые 12 часов
- Защита от rate limits Let's Encrypt

### ✅ Nginx конфигурация

- Настроен для домена volnaya-28.ru
- Автоматический редирект HTTP → HTTPS
- Проксирование на ASP.NET Core API
- Кеширование статических файлов

### ✅ Безопасность

- Docker secrets для БД паролей
- SSL/TLS 1.2 и 1.3
- OCSP stapling
- Security headers
- Непривилегированный пользователь в контейнерах

## Проверка перед запуском

### DNS настройки

```bash
nslookup volnaya-28.ru
nslookup www.volnaya-28.ru
```

Оба домена должны указывать на IP вашего сервера.

### Открытые порты

```bash
# Проверить и открыть порты
sudo ufw allow 80/tcp   # HTTP
sudo ufw allow 443/tcp  # HTTPS
sudo ufw status
```

### Установленное ПО

```bash
docker --version        # >= 20.10
docker-compose --version # >= 1.29
git --version
```

## Как работает защита от повторных SSL запросов

### 1. При docker-compose build
- ✅ Сборка только образа API
- ✅ Nginx использует готовый alpine образ
- ❌ SSL сертификат НЕ запрашивается

### 2. При docker compose up -d
- ✅ Docker Compose читает конфигурацию из .env
- ✅ Запускает все контейнеры (PostgreSQL, Web API, Nginx, Certbot)
- ✅ Nginx проверяет наличие SSL сертификата
- ✅ Если сертификат валиден (> 30 дней) - использует его
- ✅ Если сертификата нет или невалиден - автоматически получает от Let's Encrypt
- ✅ Настраивает HTTPS автоматически

### 3. Автоматическое обновление (certbot)
- ✅ Проверяет сертификаты каждые 12 часов
- ✅ Обновляет только если срок < 30 дней
- ✅ Стандартный `certbot renew` без force

## Структура проекта

```
WebSite/
├── .env                        ✅ volnaya-28.ru, admin@kalmykov-group.ru, DB credentials
├── docker-compose.yml          ✅ использует .env, автоматический SSL
├── nginx/
│   ├── conf.d/default.conf    ✅ начальная конфигурация
│   ├── docker-entrypoint.sh   ✅ проверка и получение SSL при запуске
│   └── generate-config.sh     ✅ динамическая генерация конфигурации
└── certbot/                    (создается автоматически)
    ├── conf/                   ✅ SSL сертификаты
    └── www/                    ✅ ACME challenge
```

## Способ запуска

Проект настроен для запуска одной командой:

```bash
docker compose up -d
```

Что происходит автоматически:
- ✅ Docker Compose читает конфигурацию из .env
- ✅ Запускает PostgreSQL с credentials из .env
- ✅ Запускает ASP.NET Core Web API
- ✅ Запускает Nginx с автоматической генерацией конфигурации
- ✅ Nginx проверяет наличие SSL сертификата
- ✅ Если сертификата нет или он невалиден - получает от Let's Encrypt
- ✅ Запускает Certbot для автоматического обновления сертификатов

Готово! Сайт доступен по https://volnaya-28.ru

## После развертывания

### Проверка работоспособности

```bash
# Статус контейнеров
docker-compose ps

# Логи
docker-compose logs -f

# SSL сертификат
docker-compose exec nginx openssl x509 -enddate -noout \
  -in /etc/letsencrypt/live/volnaya-28.ru/fullchain.pem

# HTTP → HTTPS редирект
curl -I http://volnaya-28.ru

# HTTPS работает
curl -I https://volnaya-28.ru
```

### Доступ к сервисам

- **Website**: https://volnaya-28.ru
- **API**: https://volnaya-28.ru/api
- **PostgreSQL**: localhost:5444 (только внутри)

### SSL оценка

Проверьте качество SSL на:
https://www.ssllabs.com/ssltest/analyze.html?d=volnaya-28.ru

Ожидается оценка: **A или A+**

## Управление

```bash
# Просмотр логов
docker-compose logs -f webapi
docker-compose logs -f nginx
docker-compose logs -f postgres

# Перезапуск
docker-compose restart

# Остановка
docker-compose down

# Обновление кода
git pull
docker compose up -d --build

# SSL обновляется автоматически через Certbot
```

## Документация

- [DEPLOY.md](DEPLOY.md) - подробная инструкция по развертыванию
- [SSL-SETUP.md](SSL-SETUP.md) - документация по SSL
- [README.md](README.md) - основная документация
- [QUICK-START.md](QUICK-START.md) - быстрый старт для разработки

## Поддержка

При возникновении проблем:

1. Проверьте логи: `docker-compose logs -f`
2. Проверьте статус: `docker-compose ps`
3. Смотрите [DEPLOY.md](DEPLOY.md) раздел "Устранение проблем"
4. Email: admin@kalmykov-group.ru

---

**Проект готов к production развертыванию!** 🚀
