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

**⚠️ ВАЖНО:** Для корректной работы системы необходимо запустить **ВСЕ** микросервисы одновременно, так как они зависят друг от друга:
- **Preferences API** (порт 8094) - требуется для GivingToCustomer и ReceivingFromPartner
- **GivingToCustomer API** (порт 8093) - требуется для ReceivingFromPartner
- **Administration API** (порт 8091) - требуется для ReceivingFromPartner
- **ReceivingFromPartner API** (порт 8092) - основной сервис

#### Вариант 1: Через F5 в VS Code (рекомендуется)

1. Нажмите **F5** в VS Code
2. Выберите одну из конфигураций:
   - **Run All Microservices (Clean, Build, Run)** - запуск всех микросервисов с полной пересборкой ⭐ **РЕКОМЕНДУЕТСЯ**
   - **Run All Microservices (Build Only)** - быстрый запуск всех микросервисов ⭐ **РЕКОМЕНДУЕТСЯ**
   - **Run Administration (Clean, Build, Run)** - запуск только Administration (не рекомендуется для полной функциональности)
   - **Run GivingToCustomer (Clean, Build, Run)** - запуск только GivingToCustomer (не рекомендуется для полной функциональности)
   - **Run ReceivingFromPartner (Clean, Build, Run)** - запуск только ReceivingFromPartner (не рекомендуется для полной функциональности)
   - **Run Preferences (Clean, Build, Run)** - запуск только Preferences (не рекомендуется для полной функциональности)

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
     - `Pcf.Preferences.WebHost` ⚠️ **ОБЯЗАТЕЛЬНО** - требуется для работы GivingToCustomer и ReceivingFromPartner
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

## Устранение проблем

### Ошибка подключения к Preferences API (localhost:8094)

Если вы получаете ошибку `Подключение не установлено, т.к. конечный компьютер отверг запрос на подключение` при обращении к порту 8094:

1. **Убедитесь, что Preferences API запущен:**
   - Проверьте, что процесс `Pcf.Preferences.WebHost` запущен
   - Проверьте доступность по адресу: http://localhost:8094/swagger

2. **Запустите все микросервисы:**
   - Используйте конфигурацию **Run All Microservices** в VS Code
   - Или запустите все проекты в Visual Studio/Rider одновременно

3. **Проверьте порядок запуска:**
   - Сначала запустите **Preferences API** (порт 8094)
   - Затем запустите остальные микросервисы

4. **Проверьте конфигурацию:**
   - Убедитесь, что в `appsettings.Development.json` указан правильный URL: `http://localhost:8094`

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

