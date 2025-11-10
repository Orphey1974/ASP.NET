# Настройка Postman для работы с gRPC (без TLS)

## Проблема: "No connection established"
Если вы видите ошибку "No connection established" в Postman при подключении к gRPC сервису, выполните следующие шаги:

## Шаг 1: Проверьте URL
Убедитесь, что вы используете **HTTP** (не HTTPS):

```
http://localhost:8093

```

## Шаг 2: Настройки Postman для gRPC без TLS

1. **Создайте новый gRPC запрос:**

   - New → gRPC Request

2. **Укажите URL:**

   - `http://localhost:8093` (обязательно HTTP, не HTTPS)

3. **Импортируйте proto файл:**

   - Нажмите "Import a .proto file"

   - Выберите: `src/Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost/Protos/customers.proto`

4. **Важно: Отключите TLS в настройках:**

   - Перейдите на вкладку **"Settings"** (внизу справа)

   - Найдите опцию **"TLS"** или **"Use TLS"**

   - **Отключите TLS** (снимите галочку)

   - Или убедитесь, что выбрано **"Plain text"** вместо **"TLS"**

5. **Выберите метод:**

   - В выпадающем списке выберите `CustomersService`

   - Выберите метод, например `GetCustomers`

6. **Нажмите "Invoke"**

## Шаг 3: Альтернатива - Использование Server Reflection
Если импорт proto файла не работает:

1. В Postman выберите **"Use Server Reflection"**

2. Укажите URL: `http://localhost:8093`

3. Postman автоматически обнаружит доступные сервисы
**Примечание:** Для Server Reflection нужно добавить поддержку в сервис (см. ниже).

## Шаг 4: Добавление Server Reflection (опционально)
Если хотите использовать Server Reflection в Postman, добавьте в `Startup.cs`:

```csharp
services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
});
// Добавьте Server Reflection для Postman
services.AddGrpcReflection();

```
И в `Configure`:

```csharp
if (env.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

```

## Проверка доступности сервиса
Перед тестированием убедитесь, что:

1. Сервис запущен: `netstat -ano | findstr :8093`

2. Сервис Preferences запущен: `netstat -ano | findstr :8094`

## Типичные ошибки

### "No connection established"

- Проверьте, что сервис запущен

- Убедитесь, что используете `http://` (не `https://`)

- Отключите TLS в настройках Postman

### "TLS required"

- Отключите TLS в настройках Postman

- Убедитесь, что URL начинается с `http://`

### "Service unavailable"

- Проверьте, что сервис запущен и слушает порт 8093

- Перезапустите сервис после изменений в коде

## Альтернативные инструменты
Если Postman не работает, попробуйте:

1. **BloomRPC** - https://github.com/bloomrpc/bloomrpc/releases

2. **grpcurl** - командная строка

3. **C# тестовый клиент** - см. `test-grpc-client.cs`
