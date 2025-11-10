# Быстрое исправление проблемы подключения gRPC в Postman

## Проблема: "No connection established"

## Решение (3 шага):

### 1. Перезапустите сервис
После изменений в коде **обязательно перезапустите** сервис `Pcf.GivingToCustomer.WebHost`:

- Остановите текущий процесс

- Пересоберите: `dotnet build`

- Запустите заново

### 2. Настройте Postman правильно
**Важно:** В Postman для gRPC запроса:

1. **URL должен быть:** `http://localhost:8093` (HTTP, не HTTPS!)

2. **Отключите TLS:**

   - Перейдите на вкладку **"Settings"** (внизу справа в Postman)

   - Найдите **"TLS"** или **"Use TLS"**

   - **Снимите галочку** или выберите **"Plain text"**

### 3. Импортируйте proto файл

1. В Postman выберите **"Import a .proto file"**

2. Выберите файл: `src/Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost/Protos/customers.proto`

3. После импорта выберите сервис `CustomersService` и нужный метод

## Альтернатива: Импорт proto файла
Если Server Reflection не работает:

1. Выберите **"Import a .proto file"**

2. Выберите: `src/Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost/Protos/customers.proto`

3. **Обязательно отключите TLS** в настройках!

## Проверка
После настройки:

- Выберите метод `GetCustomers`

- Нажмите "Invoke"

- Должен вернуться список клиентов (может быть пустым, если клиентов нет)

## Если все еще не работает

1. Проверьте, что сервис запущен: `netstat -ano | findstr :8093`

2. Убедитесь, что используете `http://` (не `https://`)

3. Перезапустите Postman

4. Попробуйте другой инструмент: BloomRPC или grpcurl
