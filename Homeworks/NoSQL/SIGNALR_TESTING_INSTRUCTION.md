# Инструкция по тестированию SignalR в сервисе Customers

## Описание
Данная инструкция описывает процесс тестирования SignalR функциональности в сервисе `Pcf.GivingToCustomer`, которая обеспечивает отправку уведомлений в реальном времени при создании, обновлении и удалении клиентов.

## Предварительные требования

1. **Запущенный сервис Pcf.GivingToCustomer**

   - Порт HTTP: `8095`

   - Порт HTTPS: `8093`

   - URL Swagger: `http://localhost:8095/swagger` или `https://localhost:8093/swagger`

2. **Браузер с поддержкой JavaScript**

   - Chrome, Firefox, Edge (рекомендуется)

   - JavaScript должен быть включен

3. **Доступ к файлу тестирования**

   - Файл: `test-signalr-customers.html` в корне проекта

## Шаг 1: Запуск сервиса

### Вариант 1: Через VS Code (F5)

1. Откройте проект в VS Code

2. Нажмите **F5**

3. Выберите конфигурацию: **"Run GivingToCustomer (Build Only)"** или **"Run GivingToCustomer (Clean, Build, Run)"**

4. Дождитесь запуска сервиса

5. Проверьте, что сервис запущен:

   - Откройте в браузере: `http://localhost:8095/swagger`

   - Должен открыться Swagger UI

### Вариант 2: Через командную строку

```powershell
cd src/Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost
dotnet run

```

### Проверка запуска
Выполните в PowerShell:

```powershell

# Проверка порта
netstat -ano | findstr :8095

# Или проверка через curl
curl http://localhost:8095/swagger/index.html

```

## Шаг 2: Открытие HTML файла для тестирования

### Вариант 1: Двойной клик

1. Найдите файл `test-signalr-customers.html` в корне проекта

2. Дважды кликните по файлу

3. Файл откроется в браузере по умолчанию

### Вариант 2: Через PowerShell

```powershell
Start-Process "test-signalr-customers.html"

```

### Вариант 3: Через браузер напрямую
В адресной строке браузера введите:

```
file:///D:/git/__Learning/Otus/__Practis/ASP.NET/Homeworks/NoSQL/test-signalr-customers.html

```
*(Замените путь на актуальный путь к файлу)*

## Шаг 3: Подключение к SignalR Hub

1. В открытом HTML файле проверьте URL сервера:

   - По умолчанию: `http://localhost:8095`

   - Если используете HTTPS: `https://localhost:8093`

2. Нажмите кнопку **"Подключиться"**

3. Ожидаемый результат:

   - Статус изменится на **"Подключено"** (зеленый фон)

   - В логе появится сообщение: `✅ Успешно подключено к SignalR Hub`

4. Если подключение не удалось:

   - Проверьте, что сервис запущен

   - Проверьте URL сервера

   - Откройте консоль браузера (F12) для просмотра ошибок

## Шаг 4: Тестирование уведомлений

### 4.1. Тестирование создания клиента

#### Через Swagger UI:

1. Откройте `http://localhost:8095/swagger`

2. Найдите endpoint `POST /api/v1/customers`

3. Нажмите **"Try it out"**

4. Введите данные клиента:

```json
{
  "email": "test@example.com",
  "firstName": "Иван",
  "lastName": "Петров",
  "preferenceIds": []
}

```

5. Нажмите **"Execute"**

#### Через PowerShell (curl):

```powershell
$body = @{
    email = "test@example.com"
    firstName = "Иван"
    lastName = "Петров"
    preferenceIds = @()
} | ConvertTo-Json
Invoke-RestMethod -Uri "http://localhost:8095/api/v1/customers" `
    -Method Post `
    -ContentType "application/json" `
    -Body $body

```

#### Ожидаемый результат:
В HTML файле должно появиться уведомление:

```
✅ Создан новый клиент: {
  "id": "...",
  "email": "test@example.com",
  "firstName": "Иван",
  "lastName": "Петров",
  ...
}

```

### 4.2. Тестирование обновления клиента

1. Получите ID созданного клиента из предыдущего шага

2. Используйте endpoint `PUT /api/v1/customers/{id}`

#### Через PowerShell:

```powershell
$customerId = "ВАШ_ID_КЛИЕНТА"
$body = @{
    email = "updated@example.com"
    firstName = "Петр"
    lastName = "Иванов"
    preferenceIds = @()
} | ConvertTo-Json
Invoke-RestMethod -Uri "http://localhost:8095/api/v1/customers/$customerId" `
    -Method Put `
    -ContentType "application/json" `
    -Body $body

```

#### Ожидаемый результат:
В HTML файле должно появиться уведомление:

```
🔄 Обновлен клиент: {
  "id": "...",
  "email": "updated@example.com",
  ...
}

```

### 4.3. Тестирование удаления клиента

#### Через PowerShell:

```powershell
$customerId = "ВАШ_ID_КЛИЕНТА"
Invoke-RestMethod -Uri "http://localhost:8095/api/v1/customers/$customerId" `
    -Method Delete

```

#### Ожидаемый результат:
В HTML файле должно появиться уведомление:

```
❌ Удален клиент: ВАШ_ID_КЛИЕНТА

```

## Шаг 5: Полный сценарий тестирования

### Сценарий 1: Создание и получение уведомления

1. Откройте HTML файл и подключитесь к SignalR

2. В другом окне браузера откройте Swagger: `http://localhost:8095/swagger`

3. Создайте клиента через Swagger

4. Проверьте, что в HTML файле появилось уведомление о создании

### Сценарий 2: Множественные операции

1. Подключитесь к SignalR

2. Выполните несколько операций подряд:

   - Создайте клиента

   - Обновите клиента

   - Удалите клиента

3. Проверьте, что все уведомления появились в правильном порядке

### Сценарий 3: Переподключение

1. Подключитесь к SignalR

2. Остановите сервис (закройте окно терминала или остановите процесс)

3. Наблюдайте за попытками переподключения в логе

4. Запустите сервис снова

5. Проверьте, что произошло автоматическое переподключение

## Устранение проблем

### Проблема 1: "Failed to complete negotiation"
**Причина:** Сервис не запущен или недоступен
**Решение:**

1. Проверьте, что сервис запущен: `netstat -ano | findstr :8095`

2. Проверьте доступность Swagger: откройте `http://localhost:8095/swagger`

3. Перезапустите сервис

### Проблема 2: "NetworkError when attempting to fetch resource"
**Причина:** Проблема с CORS или сервис не отвечает
**Решение:**

1. Проверьте конфигурацию CORS в `Startup.cs`

2. Убедитесь, что используется правильный URL (HTTP или HTTPS)

3. Проверьте консоль браузера (F12) для детальных ошибок

### Проблема 3: Уведомления не приходят
**Причина:** Не подключены обработчики событий или проблемы с SignalR
**Решение:**

1. Проверьте, что подключение установлено (статус "Подключено")

2. Проверьте консоль браузера на наличие ошибок

3. Убедитесь, что сервис отправляет уведомления (проверьте логи сервера)

4. Попробуйте переподключиться

### Проблема 4: Порт занят
**Причина:** Другой процесс использует порт 8095
**Решение:**

```powershell

# Найти процесс, использующий порт
netstat -ano | findstr :8095

# Завершить процесс (замените PID на реальный)
taskkill /F /PID <PID>

# Или используйте скрипт
.\kill-solution-processes.ps1

```

## Проверка конфигурации SignalR

### На сервере (Startup.cs)
Убедитесь, что:

1. **CORS настроен правильно:**

```csharp
services.AddCors(options =>
{
    options.AddPolicy("SignalRCorsPolicy", builder =>
    {
        builder
            .SetIsOriginAllowed(_ => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

```

2. **SignalR зарегистрирован:**

```csharp
services.AddSignalR();

```

3. **Hub замаплен:**

```csharp
endpoints.MapHub<CustomersHub>("/hubs/customers");

```

4. **CORS включен в pipeline:**

```csharp
app.UseCors("SignalRCorsPolicy");

```

### На клиенте (HTML)
Убедитесь, что:

1. **Используется правильный URL:**

   - HTTP: `http://localhost:8095/hubs/customers`

   - HTTPS: `https://localhost:8093/hubs/customers`

2. **Подключены обработчики событий:**

   - `CustomerCreated`

   - `CustomerUpdated`

   - `CustomerDeleted`

## Дополнительная информация

### Endpoints SignalR Hub

- **URL подключения:** `/hubs/customers`

- **События:**

  - `CustomerCreated` - при создании клиента

  - `CustomerUpdated` - при обновлении клиента

  - `CustomerDeleted` - при удалении клиента

  - `CustomersListUpdated` - при обновлении списка (для будущего использования)

### REST API Endpoints для тестирования

- `GET /api/v1/customers` - получить список клиентов

- `GET /api/v1/customers/{id}` - получить клиента по ID

- `POST /api/v1/customers` - создать клиента (отправляет SignalR уведомление)

- `PUT /api/v1/customers/{id}` - обновить клиента (отправляет SignalR уведомление)

- `DELETE /api/v1/customers/{id}` - удалить клиента (отправляет SignalR уведомление)

### Логирование
Для просмотра логов SignalR на сервере:

1. Откройте консоль, где запущен сервис

2. Ищите сообщения с префиксом `SignalR` или `CustomersHub`

3. Проверьте логи в `appsettings.Development.json` для настройки уровня логирования

## Заключение
После выполнения всех шагов вы должны:

1. ✅ Успешно подключиться к SignalR Hub

2. ✅ Получать уведомления при создании клиентов

3. ✅ Получать уведомления при обновлении клиентов

4. ✅ Получать уведомления при удалении клиентов

5. ✅ Видеть все события в реальном времени в HTML интерфейсе
Если все работает корректно, SignalR функциональность реализована и протестирована успешно!
