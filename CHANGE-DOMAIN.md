# Смена домена

Эта инструкция описывает как сменить домен после первоначальной настройки.

## Как это работает

При запуске nginx **автоматически генерирует** конфигурацию из шаблона, подставляя значение `DOMAIN` из переменных окружения.

```
.env файл
    ↓
docker-compose.yml читает DOMAIN
    ↓
nginx контейнер получает DOMAIN как переменную окружения
    ↓
docker-entrypoint.sh вызывает generate-config.sh
    ↓
generate-config.sh создает default.conf с новым доменом
    ↓
nginx запускается с новой конфигурацией
```

## Пошаговая инструкция

### Шаг 1: Остановите контейнеры

```bash
docker-compose down
```

### Шаг 2: Измените .env файл

```bash
nano .env
```

Измените строки:
```env
DOMAIN=новый-домен.ru
EMAIL=your@email.com
```

Сохраните файл (Ctrl+O, Enter, Ctrl+X).

### Шаг 3: Настройте DNS

Убедитесь что A-записи нового домена указывают на ваш сервер:

```bash
nslookup новый-домен.ru
nslookup www.новый-домен.ru
```

### Шаг 4: Удалите старые SSL сертификаты (опционально)

Если хотите получить новый сертификат для нового домена:

```bash
# ВНИМАНИЕ: Это удалит все сертификаты!
sudo rm -rf certbot/conf/live/*
sudo rm -rf certbot/conf/archive/*
sudo rm -rf certbot/conf/renewal/*
```

### Шаг 5: Запустите контейнеры

```bash
docker compose up -d
```

Nginx автоматически:
1. ✅ Прочитает DOMAIN из переменных окружения
2. ✅ Сгенерирует конфигурацию с новым доменом
3. ✅ Проверит наличие SSL сертификата
4. ✅ Если сертификата нет - получит новый от Let's Encrypt
5. ✅ Настроит HTTPS автоматически

### Шаг 6: Проверка

```bash
# Проверить что nginx использует новый домен
docker-compose exec nginx cat /etc/nginx/conf.d/default.conf | grep server_name

# Должно показать:
# server_name новый-домен.ru www.новый-домен.ru;

# Проверить HTTP → HTTPS редирект
curl -I http://новый-домен.ru

# Проверить HTTPS
curl -I https://новый-домен.ru

# Проверить SSL сертификат
docker-compose exec nginx openssl x509 -noout -subject -in /etc/letsencrypt/live/новый-домен.ru/fullchain.pem
```

## Альтернативный способ (без остановки)

Если хотите изменить домен без полной остановки:

```bash
# 1. Изменить .env
nano .env

# 2. Пересоздать только nginx контейнер
docker compose up -d --force-recreate nginx

# SSL настроится автоматически при запуске nginx
```

## Проверка текущего домена

```bash
# Посмотреть переменные окружения nginx контейнера
docker-compose exec nginx env | grep DOMAIN

# Посмотреть текущую конфигурацию nginx
docker-compose exec nginx cat /etc/nginx/conf.d/default.conf

# Посмотреть логи запуска nginx (показывает какой домен был использован)
docker-compose logs nginx | grep "Домен:"
```

## Откат к предыдущему домену

Если что-то пошло не так:

```bash
# 1. Остановить контейнеры
docker-compose down

# 2. Вернуть старое значение в .env
nano .env
# DOMAIN=volnaya-28.ru
# EMAIL=admin@kalmykov-group.ru

# 3. Запустить заново
docker-compose up -d

# 4. Старый SSL сертификат должен быть найден автоматически
```

## Несколько доменов одновременно

Если нужно обслуживать несколько доменов на одном сервере:

### Вариант 1: Несколько проектов

```bash
# Проект 1
cd /opt/project1
nano .env  # DOMAIN=domain1.com
docker-compose up -d

# Проект 2 (другие порты!)
cd /opt/project2
nano .env  # DOMAIN=domain2.com
nano docker-compose.yml  # Изменить порты 80→8080, 443→8443
docker-compose up -d
```

### Вариант 2: Один nginx для всех

Создать отдельный nginx с конфигурацией для нескольких доменов и проксировать на разные бэкенды. Это более сложная настройка.

## Частые ошибки

### Ошибка 1: Nginx использует старый домен

**Причина**: docker-compose не передал новую переменную окружения.

**Решение**:
```bash
docker-compose down
docker-compose up -d --force-recreate nginx
```

### Ошибка 2: SSL сертификат не выпускается

**Причина**: DNS еще не обновился или Let's Encrypt видит старый IP.

**Решение**:
```bash
# Подождите распространения DNS (до 48 часов, обычно 5-15 минут)
nslookup новый-домен.ru

# Проверьте что порт 80 открыт
sudo ufw status

# Проверьте доступность для ACME challenge
curl http://новый-домен.ru/.well-known/acme-challenge/test
```

### Ошибка 3: Старый SSL сертификат используется

**Причина**: nginx нашел старый сертификат и использует его.

**Решение**:
```bash
# Удалить старые сертификаты
sudo rm -rf certbot/conf/live/*
sudo rm -rf certbot/conf/archive/*
sudo rm -rf certbot/conf/renewal/*

# Перезапустить - новый сертификат получится автоматически
docker compose restart nginx
```

## Мониторинг после смены

```bash
# Логи nginx
docker-compose logs -f nginx

# Статус всех контейнеров
docker-compose ps

# Проверка конфигурации nginx
docker-compose exec nginx nginx -t

# Проверка переменных окружения
docker-compose exec nginx env
```

## Автоматизация

Для автоматической смены домена можно создать скрипт:

```bash
#!/bin/bash
# change-domain.sh

NEW_DOMAIN=$1
NEW_EMAIL=$2

if [ -z "$NEW_DOMAIN" ] || [ -z "$NEW_EMAIL" ]; then
    echo "Usage: ./change-domain.sh новый-домен.ru your@email.com"
    exit 1
fi

echo "Changing domain to: $NEW_DOMAIN"

# Остановить
docker compose down

# Обновить .env
sed -i "s/DOMAIN=.*/DOMAIN=$NEW_DOMAIN/" .env
sed -i "s/EMAIL=.*/EMAIL=$NEW_EMAIL/" .env

# Удалить старые сертификаты (опционально)
read -p "Delete old SSL certificates? (y/N): " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    sudo rm -rf certbot/conf/live/*
    sudo rm -rf certbot/conf/archive/*
    sudo rm -rf certbot/conf/renewal/*
fi

# Запустить - SSL настроится автоматически
docker compose up -d

echo "✅ Domain changed to: $NEW_DOMAIN"
echo "🌐 Check: https://$NEW_DOMAIN"
```

Использование:
```bash
chmod +x change-domain.sh
./change-domain.sh новый-домен.ru your@email.com
```

## Резюме

✅ **Да, смена домена работает автоматически!**

Просто измените `DOMAIN` в `.env` и перезапустите:

```bash
# Минимальная инструкция
nano .env                    # Изменить DOMAIN=
docker compose down
docker compose up -d         # Nginx автоматически подставит новый домен и настроит SSL
```

Конфигурация nginx генерируется **при каждом запуске** контейнера, а SSL сертификат получается автоматически!
