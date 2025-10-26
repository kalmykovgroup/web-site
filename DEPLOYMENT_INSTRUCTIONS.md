# Инструкция по деплою - Исправление sitemap.xml и PWA

## Проблема

При запросе `https://volnaya-28.ru/sitemap.xml` возвращалась страница 404 (index.html) вместо XML файла. Также PWA не предлагалось к установке.

## Решение

Внесены изменения в 2 компонента:

### 1. Nginx конфигурация (`nginx/generate-config.sh`)

**Что изменено:**
- Добавлены специальные location блоки для статических файлов (`.xml`, `.txt`, `.json`)
- Добавлена обработка Service Worker файлов без кэширования
- Nginx теперь передает заголовок `Accept-Static-File: true` для статических файлов

**Ключевые изменения:**

```nginx
# Важные статические файлы (sitemap, robots, manifest)
location ~* \.(xml|txt|json|webmanifest)$ {
    proxy_pass http://api_backend;
    proxy_set_header Accept-Static-File "true";
    expires 1d;
    add_header Cache-Control "public, must-revalidate";
}

# Service Worker - не кэшировать
location ~* ^/(sw\.js|registerSW\.js|workbox-.*\.js)$ {
    proxy_pass http://api_backend;
    proxy_set_header Accept-Static-File "true";
    expires -1;
    add_header Cache-Control "no-cache, no-store, must-revalidate";
}
```

### 2. ASP.NET Core приложение (`WebSite.Api/Program.cs`)

**Что изменено:**
- Заменен `MapFallbackToFile("index.html")` на умный `MapFallback` с фильтрацией
- Статические файлы теперь НЕ перенаправляются на `index.html`
- Исключены из SPA fallback: `sitemap.xml`, `robots.txt`, `manifest.json`, `sw.js`, `registerSW.js`, `workbox-*.js`

**Ключевые изменения:**

```csharp
app.MapFallback(async context =>
{
    var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";

    // Список файлов которые НЕ должны попадать под SPA fallback
    var staticFiles = new[]
    {
        "/sitemap.xml",
        "/robots.txt",
        "/manifest.json",
        "/sw.js",
        "/registersw.js",
        "/.htaccess",
        "/_redirects"
    };

    // Если запрос к статическим файлам - возвращаем 404
    if (staticFiles.Any(file => path.Equals(file, StringComparison.OrdinalIgnoreCase)))
    {
        context.Response.StatusCode = 404;
        return;
    }

    // Для остальных - отдаем index.html
    context.Request.Path = "/index.html";
    await context.Response.SendFileAsync(...);
});
```

### 3. Frontend - PWA настройка

**Что добавлено:**
- Установлен `vite-plugin-pwa`
- Настроен Service Worker с кэшированием
- Автоматическая генерация `sw.js` при сборке
- Файлы `_redirects` и `.htaccess` для разных хостингов

---

## Инструкция по деплою

### Шаг 1: Сборка Frontend

```bash
cd C:\Users\Apolon 1\WebstormProjects\website
npm run build
```

После сборки в `dist/` будут файлы:
- `sitemap.xml` ✅
- `robots.txt` ✅
- `manifest.json` ✅
- `sw.js` ✅ (Service Worker)
- `registerSW.js` ✅
- `.htaccess` ✅
- `_redirects` ✅

### Шаг 2: Копирование в Backend

Скопируйте содержимое `dist/` в `WebSite.Api/wwwroot/`:

```bash
# Удалите старые файлы
rm -rf "C:\Users\Apolon 1\source\repos\WebSite\WebSite.Api\wwwroot\*"

# Скопируйте новые
cp -r "C:\Users\Apolon 1\WebstormProjects\website\dist\*" "C:\Users\Apolon 1\source\repos\WebSite\WebSite.Api\wwwroot\"
```

### Шаг 3: Сборка Backend

```bash
cd "C:\Users\Apolon 1\source\repos\WebSite"

# Сборка .NET приложения
dotnet build --configuration Release

# Или с помощью Docker Compose
docker-compose build
```

### Шаг 4: Регенерация Nginx конфигурации

При запуске контейнера nginx автоматически запускается скрипт `generate-config.sh`, который создаст конфигурацию с учетом новых изменений.

Если нужно вручную:

```bash
docker exec -it <nginx-container-id> /docker-entrypoint.sh
```

### Шаг 5: Деплой

```bash
cd "C:\Users\Apolon 1\source\repos\WebSite"
docker-compose up -d
```

### Шаг 6: Проверка

После деплоя проверьте:

#### 1. Sitemap доступен
```bash
curl https://volnaya-28.ru/sitemap.xml
```
Должен вернуть XML, а не HTML!

#### 2. Robots.txt доступен
```bash
curl https://volnaya-28.ru/robots.txt
```

#### 3. Manifest доступен
```bash
curl https://volnaya-28.ru/manifest.json
```

#### 4. Service Worker доступен
```bash
curl https://volnaya-28.ru/sw.js
```

#### 5. PWA установка

1. Откройте https://volnaya-28.ru в Chrome/Edge
2. В адресной строке должна появиться иконка установки (+)
3. Или через меню: "Установить приложение"

#### 6. DevTools проверка

**Chrome DevTools (F12):**

1. **Application → Manifest**
   - Проверьте что manifest.json загружается
   - Должны быть иконки 192x192 и 512x512

2. **Application → Service Workers**
   - Должен быть активный Service Worker
   - Статус: "activated and is running"

3. **Network**
   - Запросите `/sitemap.xml` - должен вернуть 200 OK с Content-Type: application/xml
   - Запросите `/sw.js` - должен вернуть 200 OK с Cache-Control: no-cache

4. **Lighthouse**
   - Запустите аудит PWA
   - Должно быть зеленым: "Installable"

---

## Отправка Sitemap в поисковые системы

### Яндекс.Вебмастер

1. https://webmaster.yandex.ru/
2. Добавьте сайт `https://volnaya-28.ru`
3. Перейдите: **Индексирование → Файлы Sitemap**
4. Добавьте: `https://volnaya-28.ru/sitemap.xml`

### Google Search Console

1. https://search.google.com/search-console
2. Добавьте сайт
3. Перейдите: **Файлы Sitemap**
4. Добавьте: `https://volnaya-28.ru/sitemap.xml`

---

## Решение проблем

### Проблема: Sitemap все еще возвращает 404 или index.html

**Причины:**
1. Файл `sitemap.xml` не скопирован в `wwwroot/`
2. ASP.NET приложение не перезапущено
3. Nginx конфигурация не обновлена

**Решение:**
```bash
# 1. Проверьте наличие файла
ls "C:\Users\Apolon 1\source\repos\WebSite\WebSite.Api\wwwroot\sitemap.xml"

# 2. Пересоберите и перезапустите
docker-compose down
docker-compose build
docker-compose up -d

# 3. Проверьте логи
docker-compose logs nginx
docker-compose logs webapi
```

### Проблема: PWA не предлагается установить

**Причины:**
1. Service Worker не зарегистрирован
2. Manifest недоступен
3. Нет HTTPS (должно быть!)
4. Иконки не найдены

**Решение:**
1. Откройте DevTools → Console, проверьте ошибки
2. Откройте DevTools → Application → Manifest
3. Проверьте доступность:
   - `/manifest.json`
   - `/sw.js`
   - `/icon-192.png`
   - `/icon-512.png`
4. Очистите кэш браузера (Ctrl+Shift+Del)
5. Перезагрузите страницу (Ctrl+F5)

### Проблема: Service Worker не обновляется

**Решение:**
1. DevTools → Application → Service Workers → Unregister
2. Очистите кэш: Application → Storage → Clear site data
3. Проверьте что `sw.js` отдается с заголовком `Cache-Control: no-cache`
4. Перезагрузите страницу

---

## Файлы изменены

### Backend (WebSite)
- ✅ `nginx/generate-config.sh` - обновлена nginx конфигурация
- ✅ `WebSite.Api/Program.cs` - исправлен SPA fallback

### Frontend (website)
- ✅ `vite.config.ts` - добавлен vite-plugin-pwa
- ✅ `public/sitemap.xml` - создан sitemap
- ✅ `public/.htaccess` - конфигурация Apache
- ✅ `public/_redirects` - конфигурация Netlify
- ✅ `package.json` - добавлены зависимости PWA

### Документация
- ✅ `DEPLOYMENT.md` (frontend) - инструкция по деплою
- ✅ `DEPLOYMENT_INSTRUCTIONS.md` (backend) - эта инструкция

---

## Тестирование перед продакшеном

```bash
# 1. Локальный тест backend
cd "C:\Users\Apolon 1\source\repos\WebSite"
dotnet run --project WebSite.Api

# 2. Проверка sitemap
curl http://localhost:8080/sitemap.xml

# 3. Проверка manifest
curl http://localhost:8080/manifest.json

# 4. Проверка Service Worker
curl http://localhost:8080/sw.js
```

---

## Контрольный чеклист перед деплоем

- [ ] Frontend собран (`npm run build`)
- [ ] Файлы скопированы в `wwwroot/`
- [ ] `sitemap.xml` присутствует в `wwwroot/`
- [ ] `sw.js` и `registerSW.js` присутствуют
- [ ] Backend скомпилирован
- [ ] Docker образы пересобраны
- [ ] Nginx конфигурация обновлена
- [ ] Проверен доступ к `/sitemap.xml` локально
- [ ] Все тесты пройдены