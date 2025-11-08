# Автоматический деплой через GitHub Actions и Docker Compose

## Быстрый старт

### 1. Настройте сервер (185.207.66.77)

```bash
ssh root@185.207.66.77

# Скопируйте и запустите скрипт установки
# (содержимое из deploy/setup-server.sh)
```

### 2. Создайте SSH ключ

```bash
# На локальной машине
ssh-keygen -t ed25519 -C "github-deploy" -f github-deploy-key

# Скопируйте публичный ключ на сервер
ssh-copy-id -i github-deploy-key.pub root@185.207.66.77
```

### 3. Настройте GitHub Secrets

**Settings → Secrets and variables → Actions**

Обязательные секреты:
- `SSH_HOST` = `185.207.66.77`
- `SSH_USERNAME` = `root`
- `SSH_PORT` = `22`
- `SSH_PRIVATE_KEY` = содержимое файла `github-deploy-key`
- `DB_USER` = `webapp_user` (или другое имя)
- `DB_PASSWORD` = `strong_password_123` (ваш пароль)
- `DOMAIN` = `example.com` (или IP: `185.207.66.77`)
- `EMAIL` = `admin@example.com`
- `CORS_ORIGINS` = `https://example.com,http://localhost:5173`

### 4. Запустите деплой

```bash
git add .
git commit -m "deployment"
git push origin main
```

### 5. Проверьте результат

Откройте в браузере:
- `http://185.207.66.77`
- `http://185.207.66.77/swagger`

## Как это работает

1. **Push в main** → GitHub Actions запускается автоматически
2. **GitHub Actions** создает архив проекта
3. **Копирует** архив на сервер через SSH
4. **На сервере** автоматически:
   - Создается бэкап предыдущей версии
   - Распаковывается новая версия
   - Создаются секреты для Docker
   - Запускается `docker-compose up -d --build`

## Docker контейнеры

После деплоя на сервере будут запущены:
- `website_postgres` - PostgreSQL база данных
- `website_api` - ASP.NET Core приложение
- `website_nginx` - Nginx reverse proxy
- `website_certbot` - автообновление SSL сертификатов

## Управление на сервере

```bash
ssh root@185.207.66.77
cd /var/www/website/current

# Статус контейнеров
docker-compose ps

# Логи
docker-compose logs -f webapi

# Перезапуск
docker-compose restart webapi

# Остановка всех контейнеров
docker-compose down

# Запуск
docker-compose up -d
```

## Откат к предыдущей версии

```bash
ssh root@185.207.66.77
cd /var/www/website/current
docker-compose down
cd ..
rm -rf current
mv backup current
cd current
docker-compose up -d
```

## Полная документация

Подробное руководство: [DOCKER_DEPLOYMENT.md](./DOCKER_DEPLOYMENT.md)

## Структура файлов

```
deploy/
├── README.md                   # Этот файл (быстрый старт)
├── DOCKER_DEPLOYMENT.md        # Полное руководство
└── setup-server.sh            # Скрипт установки Docker на сервере

.github/
└── workflows/
    └── deploy.yml             # GitHub Actions workflow
```

## Troubleshooting

### GitHub Actions не может подключиться

Проверьте SSH ключ на сервере:
```bash
ssh root@185.207.66.77
cat ~/.ssh/authorized_keys
chmod 600 ~/.ssh/authorized_keys
chmod 700 ~/.ssh
```

### Контейнеры не запускаются

```bash
ssh root@185.207.66.77
cd /var/www/website/current
docker-compose logs
docker-compose down -v
docker-compose up -d --build --force-recreate
```

### База данных недоступна

```bash
docker-compose logs postgres
docker-compose restart postgres
```
