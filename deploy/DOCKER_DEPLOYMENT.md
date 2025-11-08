# Руководство по развертыванию через Docker Compose

Автоматический деплой ASP.NET Core приложения на Linux сервер через GitHub Actions и Docker Compose.

## Архитектура

Приложение разворачивается в следующих Docker контейнерах:
- **PostgreSQL** - база данных
- **WebAPI** - ASP.NET Core приложение
- **Nginx** - reverse proxy
- **Certbot** - автоматическое обновление SSL сертификатов

## Предварительные требования

### На сервере (185.207.66.77)
- Ubuntu 20.04 или новее
- Docker Engine
- Docker Compose V2

## Шаг 1: Настройка сервера

### 1.1. Подключитесь к серверу

```bash
ssh root@185.207.66.77
```

### 1.2. Запустите скрипт установки

```bash
# Скачайте скрипт (после первого push в репозиторий)
wget https://raw.githubusercontent.com/your-username/your-repo/main/deploy/setup-server.sh

# Или создайте файл вручную и скопируйте содержимое
nano setup-server.sh

# Дайте права на выполнение
chmod +x setup-server.sh

# Запустите
./setup-server.sh
```

Скрипт автоматически установит:
- Docker Engine
- Docker Compose V2
- Настроит файрвол
- Создаст необходимые директории

### 1.3. Проверьте установку

```bash
docker --version
# Должно быть: Docker version 20.10.x или новее

docker compose version
# Должно быть: Docker Compose version v2.x.x
```

## Шаг 2: Настройка SSH ключей

### 2.1. На локальной машине создайте SSH ключ

```bash
ssh-keygen -t ed25519 -C "github-deploy" -f github-deploy-key
```

Это создаст два файла:
- `github-deploy-key` (приватный ключ) - для GitHub Secrets
- `github-deploy-key.pub` (публичный ключ) - для сервера

### 2.2. Добавьте публичный ключ на сервер

**Вариант 1: Автоматически**
```bash
ssh-copy-id -i github-deploy-key.pub root@185.207.66.77
```

**Вариант 2: Вручную**
```bash
# На локальной машине - скопируйте публичный ключ
cat github-deploy-key.pub

# На сервере
ssh root@185.207.66.77
mkdir -p ~/.ssh
nano ~/.ssh/authorized_keys
# Вставьте публичный ключ, сохраните (Ctrl+X, Y, Enter)
chmod 600 ~/.ssh/authorized_keys
chmod 700 ~/.ssh
```

### 2.3. Проверьте SSH подключение

```bash
# На локальной машине
ssh -i github-deploy-key root@185.207.66.77 "echo SSH работает!"
```

## Шаг 3: Настройка GitHub Secrets

Откройте ваш репозиторий на GitHub:
**Settings → Secrets and variables → Actions → New repository secret**

Добавьте следующие секреты:

### SSH настройки

| Secret Name | Значение | Пример |
|------------|----------|--------|
| `SSH_HOST` | IP адрес сервера | `185.207.66.77` |
| `SSH_USERNAME` | Пользователь SSH | `root` |
| `SSH_PORT` | Порт SSH | `22` |
| `SSH_PRIVATE_KEY` | Приватный SSH ключ | Содержимое файла `github-deploy-key` |

Чтобы скопировать приватный ключ:

```bash
# Windows PowerShell
Get-Content github-deploy-key | Set-Clipboard

# Windows CMD
type github-deploy-key | clip

# Linux/Mac
cat github-deploy-key | pbcopy
# или просто
cat github-deploy-key
# и скопируйте вручную
```

### Настройки приложения

| Secret Name | Значение | Пример |
|------------|----------|--------|
| `DB_USER` | Пользователь PostgreSQL | `webapp_user` |
| `DB_PASSWORD` | Пароль PostgreSQL | `strong_password_123` |
| `DOMAIN` | Домен или IP | `example.com` или `185.207.66.77` |
| `EMAIL` | Email для SSL сертификата | `admin@example.com` |
| `CORS_ORIGINS` | Разрешенные CORS origins | `https://example.com,http://localhost:5173` |

## Шаг 4: Проверка конфигурации

### 4.1. Проверьте docker-compose.yml

Убедитесь, что файл `docker-compose.yml` находится в корне проекта и содержит правильную конфигурацию.

### 4.2. Проверьте Dockerfile

Убедитесь, что файл `WebSite.Api/Dockerfile` существует и настроен корректно.

## Шаг 5: Запуск деплоя

### 5.1. Сделайте commit и push

```bash
git add .
git commit -m "deployment"
git push origin main
```

### 5.2. Следите за процессом

Откройте GitHub → Actions → "Deploy to Linux Server via Docker"

Вы увидите:
1. ✅ Checkout code
2. ✅ Create deployment archive
3. ✅ Copy files to server
4. ✅ Deploy with Docker Compose

### 5.3. Что происходит при деплое:

1. **GitHub Actions** архивирует проект
2. **Копирует** архив на сервер через SCP
3. **На сервере:**
   - Создается бэкап предыдущей версии
   - Распаковывается новая версия в `/var/www/website/current`
   - Создаются файлы секретов (`secrets/db_user.txt`, `secrets/db_password.txt`)
   - Создается `.env` файл с переменными окружения
   - Останавливаются старые Docker контейнеры (`docker-compose down`)
   - Собираются и запускаются новые контейнеры (`docker-compose up -d --build`)

## Проверка работы приложения

### На сервере

```bash
ssh root@185.207.66.77

# Перейдите в директорию проекта
cd /var/www/website/current

# Проверьте статус контейнеров
docker-compose ps

# Должно быть:
# NAME                  STATUS      PORTS
# website_postgres      Up          0.0.0.0:5444->5432/tcp
# website_api          Up          8080/tcp
# website_nginx        Up          0.0.0.0:80->80/tcp, 0.0.0.0:443->443/tcp
# website_certbot      Up

# Проверьте логи
docker-compose logs -f webapi    # Логи API
docker-compose logs -f postgres  # Логи базы данных
docker-compose logs -f nginx     # Логи Nginx

# Проверьте логи конкретного контейнера
docker logs website_api --tail=100 -f
```

### В браузере

Откройте:
- `http://185.207.66.77` (или ваш домен)
- `http://185.207.66.77/swagger` - Swagger API документация

## Управление контейнерами

### Основные команды

```bash
cd /var/www/website/current

# Просмотр статуса
docker-compose ps

# Просмотр логов
docker-compose logs -f

# Перезапуск всех контейнеров
docker-compose restart

# Перезапуск конкретного контейнера
docker-compose restart webapi

# Остановка всех контейнеров
docker-compose down

# Запуск контейнеров
docker-compose up -d

# Пересборка и запуск
docker-compose up -d --build

# Просмотр использования ресурсов
docker stats
```

### Работа с базой данных

```bash
# Подключение к PostgreSQL
docker exec -it website_postgres psql -U <DB_USER> -d web_site

# Создание бэкапа базы данных
docker exec website_postgres pg_dump -U <DB_USER> web_site > backup.sql

# Восстановление из бэкапа
cat backup.sql | docker exec -i website_postgres psql -U <DB_USER> -d web_site
```

## Troubleshooting

### Контейнеры не запускаются

```bash
# Проверьте логи
docker-compose logs

# Проверьте конфигурацию
docker-compose config

# Удалите все и пересоберите
docker-compose down -v
docker-compose up -d --build --force-recreate
```

### База данных недоступна

```bash
# Проверьте, что контейнер PostgreSQL запущен
docker-compose ps postgres

# Проверьте логи PostgreSQL
docker-compose logs postgres

# Проверьте healthcheck
docker inspect website_postgres | grep -A 10 Health
```

### Приложение не отвечает

```bash
# Проверьте логи API
docker-compose logs webapi

# Перезапустите контейнер
docker-compose restart webapi

# Зайдите внутрь контейнера
docker exec -it website_api sh
```

### GitHub Actions не может подключиться

1. Проверьте SSH ключ на сервере:
   ```bash
   cat ~/.ssh/authorized_keys
   ```

2. Проверьте права:
   ```bash
   chmod 700 ~/.ssh
   chmod 600 ~/.ssh/authorized_keys
   ```

3. Проверьте GitHub Secrets:
   - Settings → Secrets and variables → Actions
   - Убедитесь, что все секреты добавлены правильно

### Ошибка "docker-compose: command not found"

```bash
# Используйте docker compose (с пробелом, V2)
docker compose version

# Если V2 не установлена, установите:
apt-get update
apt-get install docker-compose-plugin
```

## Откат к предыдущей версии

```bash
ssh root@185.207.66.77
cd /var/www/website

# Остановите текущую версию
cd current
docker-compose down

# Восстановите из бэкапа
cd ..
rm -rf current
mv backup current

# Запустите предыдущую версию
cd current
docker-compose up -d
```

## Настройка SSL (Let's Encrypt)

### Автоматическая настройка

SSL сертификаты настраиваются автоматически через Certbot, если:
1. У вас есть домен
2. Домен указывает на IP сервера
3. В GitHub Secrets указаны `DOMAIN` и `EMAIL`

### Проверка сертификата

```bash
# Проверьте статус certbot
docker-compose logs certbot

# Список сертификатов
docker exec website_certbot certbot certificates

# Ручное обновление сертификата
docker exec website_certbot certbot renew
```

## Мониторинг и логирование

### Просмотр логов в реальном времени

```bash
# Все контейнеры
docker-compose logs -f

# Только API
docker-compose logs -f webapi

# Последние 100 строк
docker-compose logs --tail=100 webapi
```

### Экспорт логов

```bash
# Сохранить логи в файл
docker-compose logs > deployment-logs.txt

# Логи за последний час
docker-compose logs --since 1h > last-hour-logs.txt
```

## Безопасность

### Рекомендации

1. **Файрвол**: Убедитесь, что открыты только необходимые порты (22, 80, 443)
2. **SSH**: Используйте ключи вместо паролей
3. **Секреты**: Никогда не коммитьте файлы из `secrets/` в Git
4. **Обновления**: Регулярно обновляйте систему и Docker
5. **Бэкапы**: Настройте автоматические бэкапы базы данных

### Закрытие прямого доступа к портам

```bash
# Только Nginx должен быть доступен извне
ufw status
ufw allow 22/tcp
ufw allow 80/tcp
ufw allow 443/tcp
ufw enable
```

## Полезные ссылки

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [Let's Encrypt Documentation](https://letsencrypt.org/docs/)
- [Nginx Documentation](https://nginx.org/ru/docs/)

## Поддержка

При возникновении проблем:
1. Проверьте логи GitHub Actions
2. Проверьте логи Docker контейнеров
3. Проверьте документацию выше
