#!/bin/bash
# Скрипт для первоначальной настройки сервера с Docker
# Запустите этот скрипт на сервере перед первым деплоем

set -e

echo "=== Настройка сервера для деплоя через Docker ==="

# Проверка, что скрипт запущен от root
if [ "$EUID" -ne 0 ]; then
    echo "Пожалуйста, запустите скрипт от root (sudo)"
    exit 1
fi

# Обновление системы
echo "Обновление системы..."
apt-get update
apt-get upgrade -y

# Установка необходимых пакетов
echo "Установка необходимых пакетов..."
apt-get install -y \
    ca-certificates \
    curl \
    gnupg \
    lsb-release

# Установка Docker
echo "Установка Docker..."
# Удаление старых версий
apt-get remove -y docker docker-engine docker.io containerd runc 2>/dev/null || true

# Добавление официального GPG ключа Docker
install -m 0755 -d /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | gpg --dearmor -o /etc/apt/keyrings/docker.gpg
chmod a+r /etc/apt/keyrings/docker.gpg

# Добавление репозитория Docker
echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
  $(lsb_release -cs) stable" | tee /etc/apt/sources.list.d/docker.list > /dev/null

# Установка Docker Engine
apt-get update
apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

# Проверка установки Docker
echo "Проверка Docker..."
docker --version
docker compose version

# Запуск Docker
systemctl start docker
systemctl enable docker

# Создание директорий
echo "Создание директорий приложения..."
mkdir -p /var/www/website/current
mkdir -p /var/www/website/backup

# Настройка файрвола
echo "Настройка файрвола..."
ufw --force enable
ufw allow 22/tcp
ufw allow 80/tcp
ufw allow 443/tcp

echo ""
echo "=== Настройка завершена ==="
echo ""
echo "Docker версии:"
docker --version
docker compose version
echo ""
echo "Следующие шаги:"
echo ""
echo "1. Добавьте публичный SSH ключ для GitHub Actions:"
echo "   mkdir -p ~/.ssh"
echo "   nano ~/.ssh/authorized_keys"
echo "   # Вставьте публичный ключ и сохраните"
echo "   chmod 600 ~/.ssh/authorized_keys"
echo "   chmod 700 ~/.ssh"
echo ""
echo "2. Настройте GitHub Secrets в репозитории:"
echo "   - SSH_HOST: IP адрес сервера (185.207.66.77)"
echo "   - SSH_USERNAME: root"
echo "   - SSH_PRIVATE_KEY: приватный SSH ключ"
echo "   - SSH_PORT: 22"
echo "   - DB_USER: имя пользователя БД"
echo "   - DB_PASSWORD: пароль БД"
echo "   - DOMAIN: ваш домен (или IP)"
echo "   - EMAIL: email для SSL сертификата"
echo "   - CORS_ORIGINS: разрешенные CORS origins"
echo ""
echo "3. Выполните первый деплой:"
echo "   git push origin main"
echo ""
echo "Готово к деплою!"
