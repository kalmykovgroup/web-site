# Быстрый старт для volnaya-28.ru

## ✅ Проект готов к запуску!

Все настроено для домена **volnaya-28.ru** с email **admin@kalmykov-group.ru**

## Запуск

```bash
# 1. Создайте секреты для базы данных
mkdir -p secrets
echo "kalmykov" > secrets/db_user.txt
echo "xK9mN2pQ7rL4wZ8vB6hT3jF5nY1dC0sA9eR7uX4kM2gH6tP8bV3qW" > secrets/db_password.txt
chmod 600 secrets/*

# 2. Запустите проект
docker compose up -d

# 3. Готово! Проверьте
curl -I http://volnaya-28.ru
```

SSL сертификат будет получен автоматически при первом запуске (если домен настроен корректно).

## Что произойдет автоматически

1. ✅ Docker Compose прочитает конфигурацию из `.env`
2. ✅ Запустит PostgreSQL с credentials из `.env`
3. ✅ Запустит ASP.NET Core Web API
4. ✅ Запустит Nginx с автоматической генерацией конфигурации
5. ✅ Nginx проверит наличие SSL сертификата
6. ✅ Если сертификата нет или он невалиден - получит новый от Let's Encrypt
7. ✅ Запустит Certbot для автоматического обновления сертификатов
8. ✅ Ваш сайт будет доступен по https://volnaya-28.ru

## Важно

Все настройки (домен, email, пароли БД) хранятся в файле `.env`:
```env
DOMAIN=volnaya-28.ru
EMAIL=admin@kalmykov-group.ru
DB_USER=kalmykov
DB_PASSWORD=...
```

**Перед первым запуском убедитесь, что DNS домена настроен** (A-запись указывает на IP сервера)

## Проверка после запуска

```bash
# Статус контейнеров
docker compose ps
# или
docker-compose ps

# Логи
docker compose logs -f
docker compose logs -f nginx

# Проверка SSL
curl -I https://volnaya-28.ru
openssl s_client -connect volnaya-28.ru:443 -servername volnaya-28.ru
```

## Важные файлы

```
.env                          # ✅ Вся конфигурация (домен, email, пароли БД)
docker-compose.yml            # ✅ Готов к запуску
nginx/
  ├── docker-entrypoint.sh   # ✅ Автоматическая настройка SSL
  └── generate-config.sh     # ✅ Генерация конфигурации nginx
```

**Примечание:** Пароли БД хранятся в `.env` - убедитесь, что файл защищен (chmod 600 .env)

## Управление

```bash
# Просмотр логов
docker compose logs -f

# Перезапуск
docker compose restart

# Остановка
docker compose down

# Обновление кода
git pull
docker compose up -d --build
```

## Смена домена

```bash
# 1. Изменить DOMAIN в .env
nano .env

# 2. Перезапустить
docker compose down
docker compose up -d

# 3. Готово! SSL получится автоматически
```

Подробнее: [CHANGE-DOMAIN.md](CHANGE-DOMAIN.md)

## Устранение проблем

### SSL не выпускается

```bash
# Проверьте DNS
nslookup volnaya-28.ru

# Проверьте порты
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp

# Проверьте доступность
curl http://volnaya-28.ru/.well-known/acme-challenge/test

# Логи
docker compose logs certbot
docker compose logs nginx
```

### База данных не подключается

```bash
# Проверьте переменные окружения
cat .env | grep DB_

# Логи PostgreSQL
docker compose logs postgres

# Подключитесь напрямую
docker compose exec postgres psql -U kalmykov -d web_site
```

## Дополнительная документация

- [DEPLOY.md](DEPLOY.md) - полная инструкция по развертыванию
- [PRODUCTION-READY.md](PRODUCTION-READY.md) - проверочный лист готовности
- [SSL-SETUP.md](SSL-SETUP.md) - детальная документация по SSL
- [CHANGE-DOMAIN.md](CHANGE-DOMAIN.md) - как сменить домен
- [README.md](README.md) - основная документация проекта

## Контакты

- **Домен**: volnaya-28.ru
- **Email**: admin@kalmykov-group.ru

## Поддержка

Если что-то не работает:

1. Проверьте логи: `docker compose logs -f`
2. Проверьте статус: `docker compose ps`
3. Смотрите раздел "Устранение проблем" в [DEPLOY.md](DEPLOY.md)

---

**Проект полностью готов к запуску! 🚀**

Просто выполните 3 команды выше и всё заработает автоматически.
