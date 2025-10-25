# ✅ Проект готов к Production развертыванию

## Домен и контакты

- **Домен**: volnaya-28.ru
- **Email**: admin@kalmykov-group.ru

## Быстрый запуск

```bash
# 1. Создайте секреты БД
mkdir secrets
echo "kalmykov" > secrets/db_user.txt
openssl rand -base64 48 > secrets/db_password.txt
chmod 600 secrets/*

# 2. Запустите в production режиме
./start-docker.sh production

# 3. Готово! Сайт доступен на https://volnaya-28.ru
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

### 2. При docker-compose up
- ✅ Nginx проверяет наличие сертификата
- ✅ Если сертификат валиден (> 30 дней) - использует его
- ✅ Если нет - работает в HTTP режиме
- ❌ Новый сертификат НЕ запрашивается

### 3. При ./start-docker.sh production
- ✅ Загружает DOMAIN и EMAIL из .env
- ✅ Запускает контейнеры
- ✅ Автоматически вызывает setup-ssl.sh
- ✅ setup-ssl.sh проверяет существующий сертификат
- ✅ Запрашивает новый ТОЛЬКО если нужно

### 4. Автоматическое обновление (certbot)
- ✅ Проверяет сертификаты каждые 12 часов
- ✅ Обновляет только если срок < 30 дней
- ✅ Стандартный `certbot renew` без force

## Структура проекта

```
WebSite/
├── .env                        ✅ volnaya-28.ru, admin@kalmykov-group.ru
├── docker-compose.yml          ✅ nginx + certbot + умный entrypoint
├── start-docker.sh             ✅ читает .env, автоматический SSL
├── setup-ssl.sh                ✅ умная проверка, без --force-renewal
├── nginx/
│   ├── conf.d/default.conf    ✅ настроен для volnaya-28.ru
│   ├── docker-entrypoint.sh   ✅ проверка SSL при запуске
│   └── generate-config.sh     ✅ динамическая генерация
├── secrets/                    ⚠️  создайте вручную
│   ├── db_user.txt
│   └── db_password.txt
└── certbot/
    ├── conf/                   ✅ SSL сертификаты (после запуска)
    └── www/                    ✅ ACME challenge
```

## Поддерживаемые способы запуска

### Способ 1: Через start-docker.sh (рекомендуется)

```bash
./start-docker.sh production
```

Автоматически:
- Читает .env
- Запускает контейнеры
- Настраивает SSL

### Способ 2: Через docker-compose

```bash
docker-compose up -d
```

Контейнеры запустятся, но SSL нужно настроить вручную:

```bash
./setup-ssl.sh
```

### Способ 3: Ручная настройка

```bash
# Запуск контейнеров
docker-compose up -d

# Настройка SSL с явными параметрами
./setup-ssl.sh volnaya-28.ru admin@kalmykov-group.ru

# Перезапуск nginx
docker-compose restart nginx
```

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
docker-compose up -d --build

# Ручное обновление SSL (если нужно)
./setup-ssl.sh
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
