# Развертывание проекта volnaya-28.ru

Это пошаговая инструкция для развертывания вашего проекта на сервере с доменом **volnaya-28.ru**.

## Предварительные требования

### 1. Настройка DNS

Убедитесь что A-записи указывают на IP вашего сервера:

```bash
# Проверка DNS
nslookup volnaya-28.ru
nslookup www.volnaya-28.ru
```

Оба домена должны указывать на IP адрес вашего сервера.

### 2. Открытые порты

Убедитесь что открыты порты:
- **80** (HTTP) - для ACME challenge и HTTP
- **443** (HTTPS) - для HTTPS трафика
- **5444** (PostgreSQL) - только для локального доступа

```bash
# Проверка портов
sudo ufw status
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
```

### 3. Установленное ПО

```bash
# Docker
docker --version

# Docker Compose
docker-compose --version

# Git
git --version
```

## Быстрое развертывание

### Шаг 1: Клонирование репозитория

```bash
cd /opt
sudo git clone <repository-url> website
cd website
sudo chown -R $USER:$USER .
```

### Шаг 2: Проверка .env файла

Файл `.env` уже настроен с правильными значениями:

```bash
cat .env
```

Должно быть:
```env
DOMAIN=volnaya-28.ru
EMAIL=admin@kalmykov-group.ru
```

✅ Файл уже в репозитории и настроен!

### Шаг 3: Создание секретов базы данных

```bash
# Создаем директорию для секретов
mkdir -p secrets

# Создаем имя пользователя БД
echo "kalmykov" > secrets/db_user.txt

# Генерируем безопасный пароль (64 символа)
openssl rand -base64 48 > secrets/db_password.txt

# Защищаем файлы
chmod 600 secrets/*

# Проверяем что создалось
ls -la secrets/
```

### Шаг 4: Запуск в production режиме

```bash
# Запускаем все контейнеры
docker compose up -d
```

**Docker Compose автоматически:**
1. ✅ Прочитает конфигурацию из `.env`
2. ✅ Соберет Docker образы
3. ✅ Запустит все контейнеры (PostgreSQL, Web API, Nginx, Certbot)
4. ✅ Nginx проверит существующий SSL сертификат
5. ✅ Если сертификата нет или он истек - получит новый от Let's Encrypt
6. ✅ Настроит HTTPS автоматически

### Шаг 5: Проверка

```bash
# Проверить что все контейнеры запущены
docker-compose ps

# Проверить логи
docker-compose logs -f

# Проверить nginx
docker-compose logs nginx

# Проверить SSL сертификат
docker-compose exec nginx ls -la /etc/letsencrypt/live/volnaya-28.ru/

# Проверить срок действия сертификата
docker-compose exec nginx openssl x509 -enddate -noout -in /etc/letsencrypt/live/volnaya-28.ru/fullchain.pem
```

### Шаг 6: Тестирование

```bash
# HTTP (должен редиректить на HTTPS)
curl -I http://volnaya-28.ru

# HTTPS
curl -I https://volnaya-28.ru

# Проверка SSL оценки
# Откройте в браузере:
# https://www.ssllabs.com/ssltest/analyze.html?d=volnaya-28.ru
```

## Доступ к сервисам

После развертывания сервисы доступны по адресам:

- **Website**: https://volnaya-28.ru
- **WWW**: https://www.volnaya-28.ru (автоматический редирект)
- **API**: https://volnaya-28.ru/api
- **Swagger**: https://volnaya-28.ru/swagger (только если ASPNETCORE_ENVIRONMENT=Development)

## Управление

### Остановка

```bash
docker-compose down
```

### Перезапуск

```bash
docker-compose restart
```

### Просмотр логов

```bash
# Все логи
docker-compose logs -f

# Только API
docker-compose logs -f webapi

# Только nginx
docker-compose logs -f nginx

# Только PostgreSQL
docker-compose logs -f postgres
```

### Обновление кода

```bash
# Получить последние изменения
git pull

# Пересобрать и перезапустить
docker-compose up -d --build
```

### Ручное обновление SSL

SSL сертификаты обновляются автоматически через Certbot каждые 12 часов.

Если нужно принудительно обновить сертификат:

```bash
# Принудительное обновление через certbot
docker compose run --rm certbot renew

# Перезапустить nginx для применения
docker compose restart nginx
```

## Автоматизация

### Автоматическое обновление SSL

Certbot уже настроен для автоматического обновления:
- Проверяет сертификаты каждые 12 часов
- Обновляет только если срок действия < 30 дней

Проверить работу certbot:
```bash
docker-compose logs certbot
```

### Резервное копирование

```bash
# Бэкап базы данных
docker-compose exec postgres pg_dump -U $(cat secrets/db_user.txt) web_site > backup_$(date +%Y%m%d).sql

# Бэкап секретов
tar -czf secrets_backup_$(date +%Y%m%d).tar.gz secrets/

# Бэкап SSL сертификатов
tar -czf ssl_backup_$(date +%Y%m%d).tar.gz certbot/conf/
```

### Cron задачи (опционально)

```bash
# Редактировать crontab
crontab -e

# Добавить задачи:

# Еженедельный бэкап БД (каждое воскресенье в 2:00)
0 2 * * 0 cd /opt/website && docker-compose exec -T postgres pg_dump -U $(cat secrets/db_user.txt) web_site > /opt/backups/db_$(date +\%Y\%m\%d).sql

# Очистка старых логов (каждый день в 3:00)
0 3 * * * docker system prune -f --filter "until=168h"
```

## Мониторинг

### Проверка здоровья

```bash
# Статус всех контейнеров
docker-compose ps

# Проверка БД
docker-compose exec postgres pg_isready -U $(cat secrets/db_user.txt)

# Проверка nginx конфигурации
docker-compose exec nginx nginx -t

# Использование ресурсов
docker stats
```

### Важные файлы для мониторинга

- `logs/` - логи приложения
- `certbot/conf/` - SSL сертификаты
- `secrets/` - учетные данные (НЕ в git)

## Устранение проблем

### SSL сертификат не выпускается

```bash
# Проверить доступность домена
curl -I http://volnaya-28.ru/.well-known/acme-challenge/test

# Проверить логи certbot
docker compose logs certbot

# Проверить логи nginx
docker compose logs nginx

# Попробовать получить сертификат вручную
docker compose run --rm certbot certonly \
  --webroot \
  --webroot-path=/var/www/certbot \
  --email admin@kalmykov-group.ru \
  --agree-tos \
  --no-eff-email \
  -d volnaya-28.ru \
  -d www.volnaya-28.ru

# Перезапустить nginx для применения
docker compose restart nginx
```

### База данных не подключается

```bash
# Проверить что контейнер запущен
docker-compose ps postgres

# Проверить логи
docker-compose logs postgres

# Проверить секреты
cat secrets/db_user.txt
cat secrets/db_password.txt

# Подключиться к БД напрямую
docker-compose exec postgres psql -U $(cat secrets/db_user.txt) -d web_site
```

### Nginx не запускается

```bash
# Проверить конфигурацию
docker-compose exec nginx nginx -t

# Проверить логи
docker-compose logs nginx

# Пересоздать контейнер
docker-compose up -d --force-recreate nginx
```

### Полный перезапуск

```bash
# Остановить все
docker-compose down

# Удалить volumes (ОСТОРОЖНО - удалит БД!)
docker-compose down -v

# Очистить SSL сертификаты (если нужно получить заново)
sudo rm -rf certbot/conf/live/*
sudo rm -rf certbot/conf/archive/*
sudo rm -rf certbot/conf/renewal/*

# Запустить заново - SSL настроится автоматически
docker compose up -d
```

## Безопасность

### Рекомендации

1. ✅ **Файлы секретов защищены** (chmod 600)
2. ✅ **SSL сертификаты обновляются автоматически**
3. ✅ **Используется HTTPS**
4. ✅ **Docker secrets для паролей БД**
5. ✅ **Nginx запускается от непривилегированного пользователя**

### Проверка безопасности

```bash
# Проверить права на секреты
ls -la secrets/

# Проверить SSL конфигурацию
docker-compose exec nginx cat /etc/nginx/conf.d/default.conf

# Сканирование портов
nmap localhost
```

## Контакты и поддержка

- **Email**: admin@kalmykov-group.ru
- **Domain**: volnaya-28.ru

## Полезные ссылки

- [README.md](README.md) - основная документация
- [SSL-SETUP.md](SSL-SETUP.md) - детальная документация по SSL
- [QUICK-START.md](QUICK-START.md) - быстрый старт для разработки
