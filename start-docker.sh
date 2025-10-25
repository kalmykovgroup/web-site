#!/bin/bash

# Скрипт запуска Docker для проекта Website
# Использование: ./start-docker.sh [production] [domain] [email]

set -e

# Загружаем переменные из .env файла если он существует
if [ -f .env ]; then
    echo "📄 Загрузка конфигурации из .env файла..."
    export $(grep -v '^#' .env | grep -v '^$' | xargs)
fi

PRODUCTION=false
DOMAIN="${DOMAIN:-yourdomain.com}"
EMAIL="${EMAIL:-your@email.com}"

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Парсинг аргументов (используем [ вместо [[ для совместимости)
if [ "$1" = "production" ]; then
    PRODUCTION=true
    DOMAIN=${2:-$DOMAIN}
    EMAIL=${3:-$EMAIL}
fi

echo -e "${CYAN}🐳 Запуск Website Docker Environment...${NC}"

# Проверка Docker
if ! command -v docker > /dev/null 2>&1; then
    echo -e "${RED}❌ Docker не установлен!${NC}"
    exit 1
fi

# Определяем команду Docker Compose (v1 или v2)
if docker compose version > /dev/null 2>&1; then
    DOCKER_COMPOSE="docker compose"
    echo -e "${GREEN}✅ Используется Docker Compose v2${NC}"
elif command -v docker-compose > /dev/null 2>&1; then
    DOCKER_COMPOSE="docker-compose"
    echo -e "${GREEN}✅ Используется Docker Compose v1${NC}"
else
    echo -e "${RED}❌ Docker Compose не установлен!${NC}"
    exit 1
fi

# Создание secrets директории
if [ ! -d "secrets" ]; then
    echo -e "${YELLOW}📁 Создание директории secrets...${NC}"
    mkdir -p secrets
fi

# Проверка secrets файлов
if [ ! -f "secrets/db_user.txt" ]; then
    echo -e "${YELLOW}📝 Создание secrets/db_user.txt...${NC}"
    echo -n "admin" > secrets/db_user.txt
fi

if [ ! -f "secrets/db_password.txt" ]; then
    echo -e "${YELLOW}📝 Создание secrets/db_password.txt...${NC}"
    echo -n "devpassword123" > secrets/db_password.txt
    echo -e "${YELLOW}⚠️  ВНИМАНИЕ: Используется дефолтный пароль! Измените в production!${NC}"
fi

if [ "$PRODUCTION" = "true" ]; then
    echo -e "${GREEN}🚀 Запуск в PRODUCTION режиме...${NC}"

    # Проверка домена в production
    if [ "$DOMAIN" = "yourdomain.com" ]; then
        echo -e "${YELLOW}⚠️  ВНИМАНИЕ: Используется дефолтный домен!${NC}"
        echo -e "${YELLOW}💡 Укажите домен: ./start-docker.sh production example.com your@email.com${NC}"
        read -p "Продолжить с дефолтным доменом? (y/N): " -n 1 -r
        echo
        if ! echo "$REPLY" | grep -iq "^y"; then
            exit 1
        fi
    fi

    # Проверка наличия nginx конфигурации
    if [ ! -f "nginx/conf.d/default.conf" ]; then
        echo -e "${RED}❌ Не найдена конфигурация nginx!${NC}"
        echo -e "${YELLOW}💡 Создайте файл nginx/conf.d/default.conf${NC}"
        exit 1
    fi

    # Production: только docker-compose.yml
    echo -e "${CYAN}🔨 Сборка контейнеров...${NC}"
    $DOCKER_COMPOSE -f docker-compose.yml build

    echo -e "${CYAN}🚀 Запуск контейнеров...${NC}"
    $DOCKER_COMPOSE -f docker-compose.yml up -d

    # Ожидание запуска контейнеров
    echo -e "${CYAN}⏳ Ожидание запуска контейнеров (10 секунд)...${NC}"
    sleep 10

    # Проверка и настройка SSL
    if [ -f "./setup-ssl.sh" ]; then
        echo -e "${CYAN}🔐 Проверка SSL сертификата...${NC}"
        chmod +x ./setup-ssl.sh
        ./setup-ssl.sh "$DOMAIN" "$EMAIL"
    else
        echo -e "${YELLOW}⚠️  Скрипт setup-ssl.sh не найден${NC}"
        echo -e "${YELLOW}💡 Запустите вручную: ./setup-ssl.sh $DOMAIN $EMAIL${NC}"
    fi

else
    echo -e "${YELLOW}🔧 Запуск в DEVELOPMENT режиме...${NC}"

    # Development: docker-compose.yml + docker-compose.override.yml
    $DOCKER_COMPOSE up -d --build
fi

# Проверка статуса
sleep 3
echo ""
echo -e "${CYAN}📊 Статус контейнеров:${NC}"
$DOCKER_COMPOSE ps

echo ""
echo -e "${GREEN}✅ Готово!${NC}"
echo ""

if [ "$PRODUCTION" = "true" ]; then
    echo -e "${CYAN}🌐 Сервисы доступны по адресам:${NC}"
    echo -e "  Website:    ${GREEN}https://$DOMAIN${NC}"
    echo -e "  API:        ${GREEN}https://$DOMAIN/api${NC}"
    echo -e "  PostgreSQL: ${GREEN}localhost:5444${NC} (только внутренний доступ)"
else
    echo -e "${CYAN}🌐 Сервисы доступны по адресам:${NC}"
    echo -e "  API:        ${GREEN}http://localhost${NC}"
    echo -e "  PostgreSQL: ${GREEN}localhost:5444${NC}"
fi

echo ""
echo -e "${CYAN}📝 Просмотр логов:${NC}"
echo "  $DOCKER_COMPOSE logs -f"
echo "  $DOCKER_COMPOSE logs -f webapi"
echo ""
echo -e "${CYAN}🛑 Остановка:${NC}"
echo "  $DOCKER_COMPOSE down"
