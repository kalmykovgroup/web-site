# Настройка SSL сертификатов

Проект включает **полностью автоматическую** настройку SSL сертификатов с использованием Let's Encrypt.

## Особенности

- ✅ **Полностью автоматическая настройка**: SSL сертификат получается при запуске контейнера
- ✅ **Умная проверка**: Сертификат запрашивается только если отсутствует или невалиден
- ✅ **Защита от повторных запросов**: Никогда не запрашивает сертификат если он валиден
- ✅ **Автоматическое обновление**: Certbot обновляет сертификаты каждые 12 часов
- ✅ **Graceful fallback**: Приложение работает по HTTP пока не получит SSL

## Быстрый старт

### 1. Для локальной разработки (без SSL)

```bash
# Убедитесь что .env настроен
cat .env

# Запустите сервисы
docker compose up -d
```

Приложение будет доступно по адресу: http://localhost

### 2. Для production (с автоматическим SSL)

#### Шаг 1: Настройте DNS

Убедитесь что A-запись вашего домена указывает на IP сервера:
```bash
# Проверка
nslookup yourdomain.com
```

#### Шаг 2: Настройте .env файл

Отредактируйте `.env`:
```env
DOMAIN=yourdomain.com
EMAIL=your@email.com
```

#### Шаг 3: Запустите приложение

```bash
docker compose up -d
```

**Готово!** SSL настроится автоматически. Ваше приложение доступно по адресу: https://yourdomain.com

## Как это работает

### При запуске nginx контейнера

Nginx использует умный entrypoint (`nginx/docker-entrypoint.sh`), который:

1. **Проверяет наличие SSL сертификата**
   ```bash
   if [ -f /etc/letsencrypt/live/*/fullchain.pem ]; then
       # Сертификат существует
   fi
   ```

2. **Проверяет срок действия** (> 30 дней)
   ```bash
   if openssl x509 -checkend 2592000 -noout -in cert.pem; then
       # Сертификат валиден
   fi
   ```

3. **Выбирает режим работы:**
   - Если сертификат валиден → использует HTTPS
   - Если сертификата нет или невалиден → получает новый от Let's Encrypt
   - Если получение не удается → работает в HTTP режиме

### Автоматическое обновление

Контейнер `certbot` работает в фоновом режиме и:
- Проверяет сертификаты каждые 12 часов
- Обновляет только если срок действия < 30 дней
- Использует стандартную команду `certbot renew` (без --force-renewal)

```yaml
certbot:
  entrypoint: "/bin/sh -c 'trap exit TERM; while :; do certbot renew --quiet; sleep 12h & wait $${!}; done;'"
```

## Проверка статуса SSL

### Проверить наличие сертификата

```bash
docker compose exec nginx ls -la /etc/letsencrypt/live/
```

### Проверить срок действия

```bash
docker compose exec nginx openssl x509 -enddate -noout -in /etc/letsencrypt/live/yourdomain.com/fullchain.pem
```

Или через внешний сервис:
```bash
echo | openssl s_client -servername yourdomain.com -connect yourdomain.com:443 2>/dev/null | openssl x509 -noout -dates
```

### Проверить конфигурацию nginx

```bash
docker compose exec nginx nginx -T
```

### Логи certbot

```bash
docker compose logs certbot
```

### Проверить SSL на внешнем сервисе

Используйте [SSL Labs](https://www.ssllabs.com/ssltest/) для полной проверки конфигурации SSL.

## Обновление сертификата вручную

SSL сертификаты обновляются автоматически, но если нужно принудительно обновить:

```bash
# Обновление через certbot (пропустит если сертификат валиден)
docker compose run --rm certbot renew

# Перезапустить nginx для применения
docker compose restart nginx
```

**Принудительное обновление (НЕ РЕКОМЕНДУЕТСЯ):**
```bash
docker compose run --rm certbot renew --force-renewal
docker compose restart nginx
```

## Получение нового сертификата вручную

Если нужно получить сертификат для другого домена:

```bash
docker compose run --rm certbot certonly \
  --webroot \
  --webroot-path=/var/www/certbot \
  --email your@email.com \
  --agree-tos \
  --no-eff-email \
  -d yourdomain.com \
  -d www.yourdomain.com

# Перезапустить nginx
docker compose restart nginx
```

## Тестирование на staging

Let's Encrypt имеет rate limits (50 сертификатов на домен в неделю). Для тестирования используйте staging:

```bash
docker compose run --rm certbot certonly \
  --webroot \
  --webroot-path=/var/www/certbot \
  --email your@email.com \
  --agree-tos \
  --staging \
  -d yourdomain.com
```

Staging сертификат не будет доверенным браузерами, но позволит протестировать процесс без риска исчерпать лимиты.

## Откат к HTTP

Если нужно временно отключить HTTPS:

```bash
# Удалите сертификаты
sudo rm -rf certbot/conf/live/*
sudo rm -rf certbot/conf/archive/*
sudo rm -rf certbot/conf/renewal/*

# Перезапустите nginx
docker compose restart nginx
```

Nginx автоматически переключится в HTTP-only режим.

## Устранение проблем

### "Certificate already exists"

Это нормально! Certbot не будет перезапрашивать сертификат если он валиден.

### "Rate limit exceeded"

Let's Encrypt имеет лимиты:
- 50 сертификатов на домен в неделю
- 5 дублирующих сертификатов в неделю

**Решение:** используйте `--staging` для тестирования

### Nginx не использует HTTPS

1. Проверьте наличие сертификата:
```bash
docker compose exec nginx ls /etc/letsencrypt/live/
```

2. Проверьте логи nginx:
```bash
docker compose logs nginx | grep -i ssl
```

3. Перезапустите nginx:
```bash
docker compose restart nginx
```

### Сертификат не получается

Проверьте:

1. **DNS настроен правильно:**
```bash
nslookup yourdomain.com
# Должен вернуть IP вашего сервера
```

2. **Порты 80 и 443 открыты:**
```bash
sudo ufw status
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
```

3. **Домен доступен извне:**
```bash
curl http://yourdomain.com/.well-known/acme-challenge/test
```

4. **Логи certbot:**
```bash
docker compose logs certbot
```

### Домен недоступен

Убедитесь что:
1. A-запись настроена правильно и DNS обновился (может занять до 48 часов)
2. Порты 80 и 443 открыты в firewall
3. Nginx запущен: `docker compose ps nginx`
4. Нет других сервисов на портах 80/443: `sudo netstat -tulpn | grep :80`

## Смена домена

При смене домена SSL настроится автоматически:

```bash
# 1. Изменить DOMAIN в .env
nano .env

# 2. Удалить старые сертификаты (опционально)
sudo rm -rf certbot/conf/live/*
sudo rm -rf certbot/conf/archive/*
sudo rm -rf certbot/conf/renewal/*

# 3. Перезапустить
docker compose down
docker compose up -d

# Готово! SSL получится автоматически для нового домена
```

## Безопасность

- ✅ Сертификаты хранятся в `certbot/conf` (исключено из git)
- ✅ Используется TLS 1.2 и 1.3
- ✅ Включен OCSP stapling
- ✅ Современные ciphers
- ✅ HSTS ready (можно включить в конфигурации nginx)

## Структура файлов

```
WebSite/
├── .env                        # DOMAIN и EMAIL
├── docker-compose.yml          # Конфигурация с nginx и certbot
├── nginx/
│   ├── nginx.conf              # Основная конфигурация
│   ├── docker-entrypoint.sh    # Автоматическая настройка SSL
│   ├── generate-config.sh      # Генератор конфигурации
│   └── conf.d/
│       └── default.conf        # Базовая конфигурация
└── certbot/                    # Создается автоматически
    ├── conf/                   # SSL сертификаты
    └── www/                    # ACME challenge
```

## Рекомендации

1. ✅ **Настройте DNS перед запуском** - A-запись должна указывать на сервер
2. ✅ **Используйте staging для тестов** - избежите rate limits
3. ✅ **Не используйте --force-renewal** без необходимости
4. ✅ **Мониторьте логи** - `docker compose logs -f certbot`
5. ✅ **Проверяйте срок действия** - хотя certbot обновляет автоматически

## Дополнительная автоматизация через cron

Хотя certbot уже настроен для автоматического обновления, можно добавить резервный cron job:

```bash
# Проверка и обновление каждую ночь в 3:00
0 3 * * * cd /path/to/project && docker compose run --rm certbot renew --quiet && docker compose restart nginx
```

## Мониторинг срока действия

Создайте скрипт для мониторинга:

```bash
#!/bin/bash
# check-ssl-expiry.sh

DOMAIN="yourdomain.com"
DAYS_WARNING=14

expiry_date=$(echo | openssl s_client -servername $DOMAIN -connect $DOMAIN:443 2>/dev/null | openssl x509 -noout -enddate | cut -d= -f2)
expiry_epoch=$(date -d "$expiry_date" +%s)
current_epoch=$(date +%s)
days_left=$(( ($expiry_epoch - $current_epoch) / 86400 ))

if [ $days_left -lt $DAYS_WARNING ]; then
    echo "WARNING: SSL certificate expires in $days_left days!"
    # Отправить уведомление
fi
```

## Ссылки

- [Let's Encrypt](https://letsencrypt.org/)
- [Certbot Documentation](https://eff-certbot.readthedocs.io/)
- [SSL Labs Test](https://www.ssllabs.com/ssltest/)
- [Mozilla SSL Configuration Generator](https://ssl-config.mozilla.org/)
