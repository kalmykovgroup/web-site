# Настройка SSL сертификатов

Этот проект включает автоматическую настройку SSL сертификатов с использованием Let's Encrypt и умную проверку валидности.

## Особенности

- ✅ **Умная проверка**: Сертификат запрашивается только если отсутствует или невалиден
- ✅ **Защита от повторных запросов**: `docker-compose build` не вызывает повторный выпуск
- ✅ **Автоматическое обновление**: Certbot автоматически обновляет сертификаты
- ✅ **Graceful fallback**: Приложение работает без SSL до его настройки

## Быстрый старт

### 1. Запуск без SSL (для локальной разработки)

```bash
# Создайте файлы с секретами БД
mkdir secrets
echo "your_username" > secrets/db_user.txt
echo "your_password" > secrets/db_password.txt

# Запустите все сервисы
docker-compose up -d
```

Приложение будет доступно по адресу: http://localhost

### 2. Настройка SSL для production

#### Шаг 1: Настройте DNS

Убедитесь что A-запись вашего домена указывает на IP сервера:
```bash
# Проверка
nslookup yourdomain.com
```

#### Шаг 2: Создайте .env файл

```bash
cp .env.example .env
```

Отредактируйте `.env`:
```env
DOMAIN=yourdomain.com
```

#### Шаг 3: Запустите приложение

```bash
docker-compose up -d
```

На этом этапе nginx запустится в режиме HTTP-only.

#### Шаг 4: Получите SSL сертификат

```bash
chmod +x setup-ssl.sh
./setup-ssl.sh yourdomain.com your@email.com
```

Скрипт автоматически:
1. Проверит наличие существующего сертификата
2. Проверит его валидность (срок действия > 30 дней)
3. Если сертификат валиден - пропустит запрос
4. Если нет - получит новый сертификат от Let's Encrypt
5. Обновит конфигурацию nginx с HTTPS
6. Перезапустит nginx

#### Шаг 5: Перезапустите nginx

```bash
docker-compose restart nginx
```

Теперь ваше приложение доступно по HTTPS: https://yourdomain.com

## Как работает защита от повторных запросов

### 1. При запуске контейнера

Nginx использует умный entrypoint (`nginx/docker-entrypoint.sh`), который:

```bash
# Проверяет наличие сертификата
if [ -f /etc/letsencrypt/live/*/fullchain.pem ]; then
    # Проверяет срок действия (> 30 дней)
    if openssl x509 -checkend 2592000 -noout -in cert.pem; then
        echo "Certificate is valid, using HTTPS"
        # Генерирует конфигурацию с HTTPS
    fi
else
    echo "No certificate, using HTTP only"
    # Генерирует конфигурацию только с HTTP
fi
```

### 2. При выполнении setup-ssl.sh

Скрипт `setup-ssl.sh` (строки 24-51):

```bash
check_certificate_validity() {
    if [ ! -f "$CERT_PATH" ]; then
        return 1  # Сертификат отсутствует
    fi

    # Проверяем срок действия
    days_until_expiry=$(...)

    if [ $days_until_expiry -lt 30 ]; then
        return 1  # Скоро истечет
    fi

    return 0  # Сертификат валиден
}

# Основная логика
if check_certificate_validity; then
    echo "SSL certificate is valid, no renewal needed"
    exit 0
fi

# Получаем новый ТОЛЬКО если проверка провалилась
obtain_certificate
```

**ВАЖНО**: В скрипте убран флаг `--force-renewal`, который игнорировал проверку!

### 3. Автоматическое обновление

Контейнер `certbot` запускается в фоне и:
- Проверяет сертификаты каждые 12 часов
- Обновляет только если срок < 30 дней
- Используется стандартная команда `certbot renew` (без force)

```yaml
certbot:
  entrypoint: "/bin/sh -c 'trap exit TERM; while :; do certbot renew --quiet; sleep 12h & wait $${!}; done;'"
```

## Проверка статуса SSL

### Проверить наличие сертификата

```bash
docker-compose exec nginx ls -la /etc/letsencrypt/live/
```

### Проверить срок действия

```bash
docker-compose exec nginx openssl x509 -enddate -noout -in /etc/letsencrypt/live/yourdomain.com/fullchain.pem
```

### Проверить конфигурацию nginx

```bash
docker-compose exec nginx nginx -T
```

### Логи certbot

```bash
docker-compose logs certbot
```

## Обновление сертификата вручную

Если нужно обновить сертификат вручную (например, для тестирования):

```bash
# Через setup-ssl.sh (с проверкой)
./setup-ssl.sh yourdomain.com your@email.com

# Или напрямую через certbot (пропустит если валиден)
docker-compose run --rm certbot renew

# Принудительное обновление (НЕ РЕКОМЕНДУЕТСЯ)
docker-compose run --rm certbot renew --force-renewal
```

После обновления перезапустите nginx:
```bash
docker-compose restart nginx
```

## Тестирование на staging

Let's Encrypt имеет rate limits. Для тестирования используйте staging:

```bash
# В setup-ssl.sh добавьте флаг --staging
--staging \
```

Staging сертификат не будет доверенным браузерами, но позволит протестировать процесс.

## Автоматизация через cron

Для полной автоматизации добавьте в crontab:

```bash
# Проверка и обновление каждую ночь в 3:00
0 3 * * * cd /path/to/project && docker-compose run --rm certbot renew --quiet && docker-compose restart nginx
```

## Откат к HTTP

Если нужно временно отключить HTTPS:

```bash
# Удалите сертификаты
sudo rm -rf certbot/conf/live/*
sudo rm -rf certbot/conf/archive/*
sudo rm -rf certbot/conf/renewal/*

# Перезапустите nginx
docker-compose restart nginx
```

Nginx автоматически переключится в HTTP-only режим.

## Устранение проблем

### "Certificate already exists"

Это нормально! Certbot не будет перезапрашивать сертификат если он валиден.

### "Rate limit exceeded"

Let's Encrypt имеет лимиты:
- 50 сертификатов на домен в неделю
- 5 дублирующих сертификатов в неделю

Решение: используйте `--staging` для тестов

### Nginx не использует HTTPS

1. Проверьте наличие сертификата:
```bash
docker-compose exec nginx ls /etc/letsencrypt/live/
```

2. Проверьте логи nginx:
```bash
docker-compose logs nginx
```

3. Перезапустите nginx:
```bash
docker-compose restart nginx
```

### Домен недоступен

Убедитесь что:
1. A-запись настроена правильно
2. Порты 80 и 443 открыты в firewall
3. Nginx запущен: `docker-compose ps nginx`

## Безопасность

- ✅ Сертификаты хранятся в `certbot/conf` (исключено из git)
- ✅ Используется TLS 1.2 и 1.3
- ✅ Включен OCSP stapling
- ✅ Современные ciphers
- ✅ HSTS ready (закомментирован по умолчанию)

## Структура файлов

```
WebSite/
├── docker-compose.yml          # Конфигурация с nginx и certbot
├── setup-ssl.sh                # Умный скрипт настройки SSL
├── .env                        # DOMAIN=yourdomain.com
├── nginx/
│   ├── nginx.conf              # Основная конфигурация
│   ├── docker-entrypoint.sh    # Умный запуск с проверкой SSL
│   ├── generate-config.sh      # Генератор конфигурации
│   └── conf.d/
│       ├── default.conf        # Базовая конфигурация (HTTP)
│       └── default.conf.template
└── certbot/
    ├── conf/                   # Сертификаты (не в git)
    └── www/                    # ACME challenge
```

## Рекомендации

1. **Всегда используйте setup-ssl.sh** вместо прямого вызова certbot
2. **Не используйте --force-renewal** без необходимости
3. **Тестируйте на staging** перед production
4. **Настройте автообновление** через cron или используйте встроенный certbot
5. **Мониторьте срок действия** сертификата

## Ссылки

- [Let's Encrypt](https://letsencrypt.org/)
- [Certbot Documentation](https://eff-certbot.readthedocs.io/)
- [SSL Labs Test](https://www.ssllabs.com/ssltest/)
