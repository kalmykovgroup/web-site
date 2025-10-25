#!/bin/bash

# Умный скрипт для управления SSL сертификатами
# Проверяет наличие и валидность сертификата
# Выпускает новый только если нужно

set -e

# Загружаем переменные из .env файла если он существует
if [ -f .env ]; then
    echo "📄 Загрузка конфигурации из .env файла..."
    export $(grep -v '^#' .env | grep -v '^$' | xargs)
fi

# Приоритет: аргументы командной строки > .env файл > значения по умолчанию
DOMAIN=${1:-${DOMAIN:-yourdomain.com}}
EMAIL=${2:-${EMAIL:-your@email.com}}
CERT_PATH="certbot/conf/live/$DOMAIN/fullchain.pem"
MIN_DAYS_VALID=30  # Обновлять если осталось меньше 30 дней

# Определяем команду Docker Compose (v1 или v2)
if docker compose version > /dev/null 2>&1; then
    DOCKER_COMPOSE="docker compose"
elif command -v docker-compose > /dev/null 2>&1; then
    DOCKER_COMPOSE="docker-compose"
else
    echo "❌ Docker Compose не установлен!"
    exit 1
fi

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}🔐 Проверка SSL сертификата для домена: $DOMAIN${NC}"

# Функция проверки валидности сертификата
check_certificate_validity() {
    if [ ! -f "$CERT_PATH" ]; then
        echo -e "${YELLOW}📝 Сертификат не найден${NC}"
        return 1
    fi

    echo -e "${BLUE}📋 Сертификат найден, проверка валидности...${NC}"

    # Получаем дату истечения
    expiry_date=$(openssl x509 -enddate -noout -in "$CERT_PATH" | cut -d= -f2)
    expiry_epoch=$(date -d "$expiry_date" +%s 2>/dev/null || date -j -f "%b %d %T %Y %Z" "$expiry_date" +%s)
    current_epoch=$(date +%s)
    days_until_expiry=$(( ($expiry_epoch - $current_epoch) / 86400 ))

    echo -e "${BLUE}📅 Сертификат истекает: $expiry_date${NC}"
    echo -e "${BLUE}⏰ Осталось дней: $days_until_expiry${NC}"

    if [ $days_until_expiry -lt 0 ]; then
        echo -e "${RED}❌ Сертификат истёк!${NC}"
        return 1
    elif [ $days_until_expiry -lt $MIN_DAYS_VALID ]; then
        echo -e "${YELLOW}⚠️  Сертификат скоро истечёт (меньше $MIN_DAYS_VALID дней)${NC}"
        return 1
    else
        echo -e "${GREEN}✅ Сертификат валиден (действителен ещё $days_until_expiry дней)${NC}"
        return 0
    fi
}

# Функция получения нового сертификата
obtain_certificate() {
    echo -e "${BLUE}📝 Получение нового SSL сертификата...${NC}"

    # Создание директорий
    mkdir -p certbot/conf
    mkdir -p certbot/www
    mkdir -p nginx/conf.d

    # Проверка что nginx запущен
    if ! $DOCKER_COMPOSE ps nginx | grep -q "Up"; then
        echo -e "${YELLOW}🚀 Запуск nginx для получения сертификата...${NC}"

        # Создаем временную конфигурацию без SSL
        cat > nginx/conf.d/default.conf << EOF
server {
    listen 80;
    listen [::]:80;
    server_name $DOMAIN www.$DOMAIN;

    location /.well-known/acme-challenge/ {
        root /var/www/certbot;
    }

    location / {
        return 200 'Waiting for SSL certificate...';
        add_header Content-Type text/plain;
    }
}
EOF

        $DOCKER_COMPOSE up -d nginx
        echo -e "${BLUE}⏳ Ожидание запуска nginx (10 секунд)...${NC}"
        sleep 10
    fi

    # Проверка доступности домена
    echo -e "${BLUE}🌐 Проверка доступности домена...${NC}"
    if ! curl -s -o /dev/null -w "%{http_code}" http://$DOMAIN/.well-known/acme-challenge/test | grep -q "200\|404"; then
        echo -e "${RED}❌ Домен недоступен! Проверьте DNS настройки.${NC}"
        echo -e "${YELLOW}💡 Убедитесь что A-запись $DOMAIN указывает на этот сервер${NC}"
        exit 1
    fi

    # Получение сертификата
    echo -e "${BLUE}🎫 Запрос сертификата от Let's Encrypt...${NC}"

    if $DOCKER_COMPOSE run --rm certbot certonly \
        --webroot \
        --webroot-path=/var/www/certbot \
        --email $EMAIL \
        --agree-tos \
        --no-eff-email \
        -d $DOMAIN \
        -d www.$DOMAIN; then

        echo -e "${GREEN}✅ Сертификат успешно получен!${NC}"
        return 0
    else
        echo -e "${RED}❌ Ошибка получения сертификата!${NC}"
        return 1
    fi
}

# Функция применения конфигурации с SSL
apply_ssl_config() {
    echo -e "${BLUE}📝 Применение конфигурации nginx с SSL...${NC}"

    # Восстанавливаем полную конфигурацию с SSL
    # (предполагается что файл nginx/conf.d/default.conf уже содержит SSL секцию)

    # Проверка конфигурации
    if $DOCKER_COMPOSE exec nginx nginx -t 2>&1 | grep -q "successful"; then
        echo -e "${GREEN}✅ Конфигурация nginx валидна${NC}"
        echo -e "${BLUE}🔄 Перезапуск nginx...${NC}"
        $DOCKER_COMPOSE restart nginx
        echo -e "${GREEN}✅ Nginx перезапущен с SSL${NC}"
    else
        echo -e "${RED}❌ Ошибка в конфигурации nginx!${NC}"
        $DOCKER_COMPOSE exec nginx nginx -t
        return 1
    fi
}

# Основная логика
main() {
    # Проверка аргументов
    if [ "$DOMAIN" = "yourdomain.com" ]; then
        echo -e "${YELLOW}⚠️  Предупреждение: используется дефолтный домен!${NC}"
        echo -e "${YELLOW}💡 Использование: ./setup-ssl.sh your-domain.com your@email.com${NC}"
        read -p "Продолжить с доменом 'yourdomain.com'? (y/N): " -n 1 -r
        echo
        if ! echo "$REPLY" | grep -iq "^y"; then
            exit 1
        fi
    fi

    # Проверка существующего сертификата
    if check_certificate_validity; then
        echo -e "${GREEN}✅ SSL сертификат актуален, обновление не требуется${NC}"

        # Применяем конфигурацию если nginx не использует SSL
        if ! $DOCKER_COMPOSE exec nginx nginx -T 2>&1 | grep -q "ssl_certificate.*$DOMAIN"; then
            echo -e "${YELLOW}⚠️  Nginx не использует SSL конфигурацию${NC}"
            apply_ssl_config
        fi

        exit 0
    fi

    # Получаем новый сертификат
    echo -e "${YELLOW}📝 Требуется получение/обновление сертификата${NC}"

    if obtain_certificate; then
        apply_ssl_config

        echo ""
        echo -e "${GREEN}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
        echo -e "${GREEN}✅ SSL успешно настроен!${NC}"
        echo -e "${GREEN}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
        echo ""
        echo -e "${BLUE}🌐 Ваш сайт доступен по адресу:${NC}"
        echo -e "   ${GREEN}https://$DOMAIN${NC}"
        echo -e "   ${GREEN}https://www.$DOMAIN${NC}"
        echo ""
        echo -e "${BLUE}📅 Автообновление сертификата:${NC}"
        echo -e "   Certbot автоматически обновит сертификат через контейнер"
        echo -e "   Или добавьте в cron:"
        echo -e "   ${YELLOW}0 0 * * * cd $(pwd) && $DOCKER_COMPOSE run --rm certbot renew && $DOCKER_COMPOSE restart nginx${NC}"
        echo ""
    else
        echo -e "${RED}❌ Не удалось настроить SSL${NC}"
        exit 1
    fi
}

# Запуск
main
