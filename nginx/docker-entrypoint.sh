#!/bin/sh
# Умный entrypoint для nginx
# Автоматически проверяет наличие SSL и генерирует конфигурацию

set -e

echo "🚀 Starting Nginx with smart SSL detection..."

# Переменные окружения
DOMAIN=${DOMAIN:-localhost}
AUTO_GENERATE_CONFIG=${AUTO_GENERATE_CONFIG:-true}

# Путь к сертификату
CERT_DIR="/etc/letsencrypt/live"
CERT_FOUND=false

# Проверяем наличие любых сертификатов
if [ -d "$CERT_DIR" ]; then
    for domain_dir in "$CERT_DIR"/*; do
        if [ -d "$domain_dir" ]; then
            CERT_PATH="$domain_dir/fullchain.pem"
            KEY_PATH="$domain_dir/privkey.pem"

            if [ -f "$CERT_PATH" ] && [ -f "$KEY_PATH" ]; then
                echo "✅ SSL certificate found in: $domain_dir"

                # Проверяем срок действия
                if openssl x509 -checkend 2592000 -noout -in "$CERT_PATH" > /dev/null 2>&1; then
                    echo "✅ Certificate is valid for at least 30 days"
                    CERT_FOUND=true

                    # Извлекаем домен из пути
                    FOUND_DOMAIN=$(basename "$domain_dir")
                    export DOMAIN="$FOUND_DOMAIN"
                else
                    echo "⚠️  Certificate expires soon (less than 30 days)"
                    CERT_FOUND=true
                fi
                break
            fi
        fi
    done
fi

# Генерируем конфигурацию ВСЕГДА (чтобы подставить актуальный DOMAIN)
echo "🔧 Generating nginx configuration for domain: $DOMAIN"
if [ -f "/etc/nginx/generate-config.sh" ]; then
    sh /etc/nginx/generate-config.sh || {
        echo "⚠️  Config generation failed, using existing config"
        echo "⚠️  Make sure DOMAIN environment variable is set correctly"
    }
else
    echo "⚠️  generate-config.sh not found, using existing config"
    echo "⚠️  Domain in config may not match DOMAIN=$DOMAIN"
fi

# Информация о режиме работы
if [ "$CERT_FOUND" = "true" ]; then
    echo "✅ Starting nginx with HTTPS support for domain: $DOMAIN"
else
    echo "⚠️  Starting nginx in HTTP-only mode"
    echo "💡 To enable HTTPS, run: ./setup-ssl.sh $DOMAIN your@email.com"
fi

# Тестируем конфигурацию
echo "🔍 Testing nginx configuration..."
if ! nginx -t; then
    echo "❌ Nginx configuration test failed!"
    echo "📋 Current configuration:"
    cat /etc/nginx/conf.d/default.conf
    exit 1
fi

echo "✅ Nginx configuration is valid"

# Запускаем nginx
echo "🎯 Starting nginx..."
exec nginx -g 'daemon off;'
