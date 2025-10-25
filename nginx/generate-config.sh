#!/bin/sh
# Умный генератор nginx конфигурации
# Создает конфигурацию в зависимости от наличия SSL сертификата

set -e

DOMAIN=${DOMAIN:-localhost}
CERT_PATH="/etc/letsencrypt/live/${DOMAIN}/fullchain.pem"
KEY_PATH="/etc/letsencrypt/live/${DOMAIN}/privkey.pem"
TEMPLATE="/etc/nginx/conf.d/default.conf.template"
OUTPUT="/etc/nginx/conf.d/default.conf"

echo "🔧 Генерация nginx конфигурации для домена: ${DOMAIN}"

# Проверяем наличие SSL сертификата
if [ -f "$CERT_PATH" ] && [ -f "$KEY_PATH" ]; then
    echo "✅ SSL сертификат найден, создаю конфигурацию с HTTPS"

    # HTTP редирект на HTTPS
    HTTP_LOCATION="return 301 https://\$server_name\$request_uri;"

    # HTTPS сервер
    HTTPS_SERVER="server {
    listen 443 ssl http2;
    listen [::]:443 ssl http2;
    server_name ${DOMAIN} www.${DOMAIN};

    # SSL Configuration
    ssl_certificate ${CERT_PATH};
    ssl_certificate_key ${KEY_PATH};
    ssl_session_timeout 1d;
    ssl_session_cache shared:SSL:50m;
    ssl_session_tickets off;

    # Modern SSL configuration
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers ECDHE-RSA-AES128-GCM-SHA256:ECDHE-RSA-AES256-GCM-SHA384:ECDHE-RSA-CHACHA20-POLY1305;
    ssl_prefer_server_ciphers off;

    # HSTS (опционально, раскомментируйте после тестирования)
    # add_header Strict-Transport-Security \"max-age=31536000; includeSubDomains\" always;

    # OCSP Stapling
    ssl_stapling on;
    ssl_stapling_verify on;
    ssl_trusted_certificate ${CERT_PATH};

    # Proxy to API
    location / {
        proxy_pass http://api_backend;
        proxy_http_version 1.1;

        # Headers
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
        proxy_set_header X-Forwarded-Host \$server_name;

        # Timeouts
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;

        # Buffering
        proxy_buffering off;
        proxy_cache_bypass \$http_upgrade;
    }

    # Static files caching
    location ~* \.(jpg|jpeg|png|gif|ico|css|js|svg|woff|woff2|ttf|eot)$ {
        proxy_pass http://api_backend;
        proxy_cache_valid 200 1d;
        expires 1d;
        add_header Cache-Control \"public, immutable\";
    }
}"
else
    echo "⚠️  SSL сертификат не найден, создаю конфигурацию только для HTTP"

    # Проксируем напрямую без редиректа
    HTTP_LOCATION="proxy_pass http://api_backend;
        proxy_http_version 1.1;

        # Headers
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;

        # Timeouts
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;

        # Buffering
        proxy_buffering off;
        proxy_cache_bypass \$http_upgrade;"

    # Нет HTTPS сервера
    HTTPS_SERVER=""
fi

# Создаем конфигурацию из template
if [ -f "$TEMPLATE" ]; then
    sed -e "s|\${DOMAIN}|${DOMAIN}|g" \
        -e "s|# SSL_REDIRECT_PLACEHOLDER|${HTTP_LOCATION}|g" \
        -e "s|# SSL_SERVER_PLACEHOLDER|${HTTPS_SERVER}|g" \
        "$TEMPLATE" > "$OUTPUT"
else
    # Если template нет, создаем базовую конфигурацию
    cat > "$OUTPUT" << EOF
upstream api_backend {
    server webapi:8080;
    keepalive 32;
}

server {
    listen 80;
    listen [::]:80;
    server_name ${DOMAIN} www.${DOMAIN};

    location /.well-known/acme-challenge/ {
        root /var/www/certbot;
        try_files \$uri =404;
    }

    location / {
        ${HTTP_LOCATION}
    }
}

${HTTPS_SERVER}
EOF
fi

echo "✅ Конфигурация создана: ${OUTPUT}"

# Проверяем конфигурацию
if nginx -t 2>&1; then
    echo "✅ Конфигурация nginx валидна"
else
    echo "❌ Ошибка в конфигурации nginx!"
    exit 1
fi
