# NoSQL Integration - MongoDB + Redis
Этот проект демонстрирует полную миграцию микросервиса Администрирование с PostgreSQL на MongoDB и добавление микросервиса предпочтений с Redis кэшированием.

# Otus.PromoCodeFactory
Проект для домашних заданий и демо по курсу `C# ASP.NET Core Разработчик` от `Отус`.
Cистема `Promocode Factory` для выдачи промокодов партнеров для клиентов по группам предпочтений.

## 🆕 Новый микросервис: Pcf.Preferences
Добавлен микросервис для управления справочником предпочтений с поддержкой Redis кэширования:

- **Порт**: 8094

- **База данных**: SQLite

- **Кэш**: Redis (продакшн) / In-Memory (разработка)

- **API**: RESTful для CRUD операций с предпочтениями

## Что реализовано

### Полная миграция на MongoDB

- ✅ Удалены все зависимости Entity Framework

- ✅ Удален PostgreSQL из docker-compose

- ✅ Удалены EF репозитории и контекст

- ✅ Созданы модели для MongoDB (EmployeeDocument, RoleDocument)

- ✅ Реализованы репозитории для работы с MongoDB

- ✅ Обновлены контроллеры для работы только с MongoDB

- ✅ Настроен DI контейнер только для MongoDB

- ✅ Добавлен инициализатор данных для MongoDB

### Архитектура (только MongoDB)

- **Документы MongoDB**: `EmployeeDocument`, `RoleDocument`

- **Репозитории**: `MongoEmployeeRepository`, `MongoRoleRepository`

- **Мапперы**: `EmployeeMapper`, `RoleMapper`

- **Контекст**: `MongoDbContext`

- **Инициализатор**: `MongoDbInitializer`

## Запуск проекта
**⚠️ ВАЖНО:** Для корректной работы системы необходимо запустить **ВСЕ** микросервисы одновременно. Подробная инструкция по запуску находится в файле [START_INSTRUCTION.md](START_INSTRUCTION.md).

### Быстрый старт

1. Запустите базы данных: `.\start-databases.ps1`

2. В VS Code нажмите **F5** и выберите **Run All Microservices (Build Only)**

### 1. Запуск только MongoDB

```bash
docker-compose up promocode-factory-administration-mongodb

```

### 2. Запуск всех сервисов

```bash
docker-compose up

```

### 3. Запуск через Visual Studio/Rider

1. Запустите MongoDB (команда выше)

2. Откройте решение `Pcf.Administration.sln`

3. Запустите проект `Pcf.Administration.WebHost`

## API Endpoints

### Сотрудники (MongoDB) - Порт 8091

- `GET /api/v1/employees` - Получить всех сотрудников

- `GET /api/v1/employees/{id}` - Получить сотрудника по ID

- `POST /api/v1/employees/{id}/appliedPromocodes` - Обновить количество выданных промокодов

### Роли (MongoDB) - Порт 8091

- `GET /api/v1/roles` - Получить все роли

### Предпочтения (SQLite + Redis) - Порт 8094

- `GET /api/v1/preferences` - Получить все предпочтения

- `GET /api/v1/preferences/{id}` - Получить предпочтение по ID

- `POST /api/v1/preferences` - Создать новое предпочтение

- `PUT /api/v1/preferences/{id}` - Обновить предпочтение

- `DELETE /api/v1/preferences/{id}` - Удалить предпочтение

## Конфигурация

### MongoDB настройки

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://admin:password@localhost:27017",
    "DatabaseName": "promocode_factory_administration"
  }
}

```

### Docker Compose

- **MongoDB**: порт 27017

- **Redis**: порт 6379

- **Administration API**: порт 8091

- **Preferences API**: порт 8094

## Тестирование

### Administration API (MongoDB)

1. Откройте Swagger UI: `https://localhost:8091/swagger`

2. Протестируйте endpoints для сотрудников и ролей

3. Данные будут сохраняться в MongoDB

### Preferences API (SQLite + Redis)

1. Запустите микросервис: `dotnet run --project src/Pcf.Preferences/Pcf.Preferences.WebHost`

2. Протестируйте API: `http://localhost:8094/api/v1/preferences`

3. Используйте тестовый скрипт: `./test-preferences-api.ps1`

## Структура данных

### EmployeeDocument

```json
{
  "_id": "ObjectId",
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "roleId": "ObjectId",
  "appliedPromocodesCount": "number",
  "fullName": "string" // вычисляемое поле
}

```

### RoleDocument

```json
{
  "_id": "ObjectId",
  "name": "string",
  "description": "string"
}

```

### Preference (SQLite)

```json
{
  "Id": "Guid",
  "Name": "string",
  "Description": "string"
}

```

## Особенности реализации

1. **Полная миграция**: Полностью удален Entity Framework и PostgreSQL из Administration

2. **MongoDB**: Administration работает исключительно с MongoDB

3. **Redis кэширование**: Preferences использует Redis для быстрого доступа к данным

4. **Гибкое кэширование**: Поддержка как Redis, так и in-memory кэша

5. **Маппинг**: Используются мапперы для преобразования между доменными моделями и документами

6. **Упрощение**: Для демонстрации используются упрощенные запросы

7. **Инициализация**: Автоматическая инициализация тестовых данных

## Следующие шаги
Для production использования рекомендуется:

1. Реализовать более эффективные запросы к MongoDB

2. Добавить индексы для оптимизации

3. Реализовать правильное управление ObjectId

4. Добавить валидацию данных

5. Реализовать миграции данных

6. Добавить логирование и мониторинг

7. **Интеграция микросервисов**: Обновить GivingToCustomer и ReceivingFromPartner для использования нового сервиса предпочтений

8. **Мониторинг кэша**: Добавить метрики для Redis кэша

9. **Безопасность**: Добавить аутентификацию и авторизацию
