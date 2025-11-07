# Инструкция по запуску проекта

## Быстрый старт

### 1. Запуск баз данных

Для запуска баз данных и инфраструктурных сервисов выполните:

```powershell
.\start-databases.ps1
```

Или вручную через Docker Compose:

```bash
docker-compose up -d promocode-factory-administration-mongodb promocode-factory-receiving-from-partner-db promocode-factory-giving-to-customer-db promocode-factory-redis promocode-factory-rabbitmq
```

**Запускаемые сервисы:**
- MongoDB для Administration (localhost:27017)
- PostgreSQL для ReceivingFromPartner (localhost:5434)
- PostgreSQL для GivingToCustomer (localhost:5435)
- Redis для Preferences (localhost:6379)
- RabbitMQ для обмена сообщениями (localhost:5672, Management UI: http://localhost:15672)

### 2. Запуск микросервисов

#### Вариант 1: Через F5 в VS Code (рекомендуется)

1. Нажмите **F5** в VS Code
2. Выберите одну из конфигураций:
   - **Run All Microservices (Clean, Build, Run)** - запуск всех микросервисов с полной пересборкой
   - **Run All Microservices (Build Only)** - быстрый запуск всех микросервисов
   - **Run Administration (Clean, Build, Run)** - запуск только Administration
   - **Run GivingToCustomer (Clean, Build, Run)** - запуск только GivingToCustomer
   - **Run ReceivingFromPartner (Clean, Build, Run)** - запуск только ReceivingFromPartner
   - **Run Preferences (Clean, Build, Run)** - запуск только Preferences

**Примечание:** При запуске через F5 базы данных запускаются автоматически через preLaunchTask.

#### Вариант 2: Через Visual Studio или Rider

1. Откройте решение `src/Pcf.sln`
2. Настройте запуск нескольких проектов одновременно:
   - Правой кнопкой на решение → **Set Startup Projects**
   - Выберите **Multiple startup projects**
   - Установите **Start** для:
     - `Pcf.Administration.WebHost`
     - `Pcf.GivingToCustomer.WebHost`
     - `Pcf.ReceivingFromPartner.WebHost`
     - `Pcf.Preferences.WebHost` (опционально)
3. Нажмите **F5** для запуска

#### Вариант 3: Через командную строку

```powershell
# Сборка решения
dotnet build src/Pcf.sln

# Запуск каждого микросервиса в отдельном терминале
dotnet run --project src/Pcf.Administration/Pcf.Administration.WebHost
dotnet run --project src/Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost
dotnet run --project src/Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.WebHost
dotnet run --project src/Pcf.Preferences/Pcf.Preferences.WebHost
```

## Порты микросервисов

- **Administration API**: http://localhost:8091/swagger
- **GivingToCustomer API**: http://localhost:8093/swagger
- **ReceivingFromPartner API**: http://localhost:8092/swagger
- **Preferences API**: http://localhost:8094/swagger

## Работа с API

Все API имеют Swagger UI с примерами данных для тестирования. Swagger автоматически открывается при запуске через F5.

## Остановка баз данных

Для остановки всех контейнеров выполните:

```bash
docker-compose down
```

Для остановки только баз данных (без удаления данных):

```bash
docker-compose stop promocode-factory-administration-mongodb promocode-factory-receiving-from-partner-db promocode-factory-giving-to-customer-db promocode-factory-redis promocode-factory-rabbitmq
```

## Требования

- Docker и Docker Compose
- .NET 8.0 SDK
- Visual Studio 2022 / Rider / VS Code с расширением C# Dev Kit

