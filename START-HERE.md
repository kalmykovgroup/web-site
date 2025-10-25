# Быстрый старт для volnaya-28.ru

## ✅ Проект готов к запуску!

Все настроено для домена **volnaya-28.ru** с email **admin@kalmykov-group.ru**

## 3 команды для запуска

```bash
# 1. Создайте секреты БД
mkdir -p secrets
echo "kalmykov" > secrets/db_user.txt
openssl rand -base64 48 > secrets/db_password.txt
chmod 600 secrets/*

# 2. Запустите в production режиме
chmod +x start-docker.sh setup-ssl.sh
./start-docker.sh production

# 3. Готово! Проверьте
curl -I https://volnaya-28.ru
```

## Что произойдет автоматически

1. ✅ Скрипт определит версию Docker Compose (v1 или v2)
2. ✅ Прочитает домен и email из `.env`
3. ✅ Соберет и запустит все контейнеры
4. ✅ Проверит существующий SSL сертификат
5. ✅ Если нужно - получит новый от Let's Encrypt
6. ✅ Настроит nginx с HTTPS
7. ✅ Ваш сайт будет доступен по https://volnaya-28.ru

## Поддержка Docker Compose v2

Скрипты автоматически определяют версию:
- `docker compose` (v2) - новая версия ✅
- `docker-compose` (v1) - старая версия ✅

Оба варианта работают!

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
.env                          # ✅ DOMAIN=volnaya-28.ru настроено
docker-compose.yml            # ✅ готов к запуску
start-docker.sh               # ✅ без BOM, поддержка v1/v2
setup-ssl.sh                  # ✅ без BOM, умная проверка
secrets/                      # ⚠️ создайте вручную
  ├── db_user.txt            # kalmykov
  └── db_password.txt        # безопасный пароль
```

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

# Ручное обновление SSL
./setup-ssl.sh
```

## Смена домена

```bash
# 1. Изменить .env
nano .env

# 2. Перезапустить
docker compose down
docker compose up -d

# 3. Новый SSL
./setup-ssl.sh

# 4. Готово!
```

Подробнее: [CHANGE-DOMAIN.md](CHANGE-DOMAIN.md)

## Устранение проблем

### Ошибка: "#!/bin/bash: not found"

**Исправлено!** Скрипты пересозданы без BOM.

### Ошибка: "docker-compose: command not found"

**Исправлено!** Скрипты теперь поддерживают обе версии:
- `docker compose` (v2)
- `docker-compose` (v1)

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
# Проверьте секреты
cat secrets/db_user.txt
cat secrets/db_password.txt

# Логи PostgreSQL
docker compose logs postgres

# Подключитесь напрямую
docker compose exec postgres psql -U $(cat secrets/db_user.txt) -d web_site
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
