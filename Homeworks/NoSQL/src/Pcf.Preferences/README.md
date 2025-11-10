# Pcf.Preferences - Микросервис предпочтений
Микросервис для управления справочником предпочтений клиентов с поддержкой Redis кэширования.

## 🏗️ Архитектура

- **Core** - Доменные модели и интерфейсы

- **DataAccess** - Репозитории, контекст БД и сервисы кэширования

- **WebHost** - API контроллеры и конфигурация

## 🚀 Возможности

- ✅ CRUD операции с предпочтениями

- ✅ Redis кэширование для быстрого доступа

- ✅ In-memory кэш для локальной разработки

- ✅ SQLite база данных

- ✅ RESTful API

- ✅ Docker поддержка

## 📋 API Endpoints

### Предпочтения

- `GET /api/v1/preferences` - Получить все предпочтения

- `GET /api/v1/preferences/{id}` - Получить предпочтение по ID

- `POST /api/v1/preferences` - Создать новое предпочтение

- `PUT /api/v1/preferences/{id}` - Обновить предпочтение

- `DELETE /api/v1/preferences/{id}` - Удалить предпочтение

## 🛠️ Запуск

### Локальная разработка

```bash

# Перейти в папку проекта
cd src/Pcf.Preferences

# Собрать проект
dotnet build

# Запустить с in-memory кэшем
dotnet run --project Pcf.Preferences.WebHost --urls "http://localhost:8094"

```

### Docker

```bash

# Запустить все сервисы
docker-compose up

# Или только микросервис предпочтений
docker-compose up promocode-factory-preferences-api

```

## ⚙️ Конфигурация

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=preferences.db",
    "Redis": "localhost:6379"
  },
  "CacheSettings": {
    "UseRedis": false
  }
}

```

### Переменные окружения

- `ConnectionStrings:DefaultConnection` - Строка подключения к БД

- `ConnectionStrings:Redis` - Строка подключения к Redis

- `CacheSettings:UseRedis` - Использовать Redis (true/false)

## 🗄️ База данных
Используется SQLite с Entity Framework Core. При первом запуске автоматически создается база данных и заполняется тестовыми данными:

- Электроника

- Одежда

- Книги

- Спорт

- Красота

## 🔄 Кэширование

### Redis (продакшн)

- Распределенный кэш

- Персистентность данных

- Масштабируемость

### In-Memory (разработка)

- Локальный кэш

- Быстрый доступ

- Автоматическая очистка

## 📦 Зависимости

- .NET 8.0

- Entity Framework Core

- StackExchange.Redis

- Microsoft.Extensions.Caching.*

## 🧪 Тестирование

```bash

# Запустить тестовый скрипт
./test-preferences-api.ps1

```

## 🔗 Интеграция
Микросервис интегрируется с другими сервисами через HTTP API:

- `GivingToCustomer` - получение предпочтений для выдачи промокодов

- `ReceivingFromPartner` - получение предпочтений при обработке промокодов от партнеров

## 📝 Примеры использования

### Получение всех предпочтений

```bash
curl -X GET "http://localhost:8094/api/v1/preferences"

```

### Создание предпочтения

```bash
curl -X POST "http://localhost:8094/api/v1/preferences" \
  -H "Content-Type: application/json" \
  -d '{"name": "Новое предпочтение", "description": "Описание"}'

```
