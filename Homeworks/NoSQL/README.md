# NoSQL Integration - MongoDB

Этот проект демонстрирует полную миграцию микросервиса Администрирование с PostgreSQL на MongoDB.
# Otus.PromoCodeFactory

Проект для домашних заданий и демо по курсу `C# ASP.NET Core Разработчик` от `Отус`.
Cистема `Promocode Factory` для выдачи промокодов партнеров для клиентов по группам предпочтений.


Данный проект является стартовой точкой для домашнего задания по NoSQL.

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

### Сотрудники (MongoDB)
- `GET /api/v1/employees` - Получить всех сотрудников
- `GET /api/v1/employees/{id}` - Получить сотрудника по ID
- `POST /api/v1/employees/{id}/appliedPromocodes` - Обновить количество выданных промокодов

### Роли (MongoDB)
- `GET /api/v1/roles` - Получить все роли

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
- **API**: порт 8091

## Тестирование

1. Откройте Swagger UI: `https://localhost:8091/swagger`
2. Протестируйте endpoints для сотрудников и ролей
3. Данные будут сохраняться в MongoDB

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

## Особенности реализации

1. **Полная миграция**: Полностью удален Entity Framework и PostgreSQL
2. **Только MongoDB**: Вся функциональность работает исключительно с MongoDB
3. **Маппинг**: Используются мапперы для преобразования между доменными моделями и MongoDB документами
4. **Упрощение**: Для демонстрации используются упрощенные запросы
5. **Инициализация**: Автоматическая инициализация тестовых данных

## Следующие шаги

Для production использования рекомендуется:
1. Реализовать более эффективные запросы к MongoDB
2. Добавить индексы для оптимизации
3. Реализовать правильное управление ObjectId
4. Добавить валидацию данных
5. Реализовать миграции данных
6. Добавить логирование и мониторинг