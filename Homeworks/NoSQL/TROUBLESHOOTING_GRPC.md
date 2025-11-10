# Устранение проблем с подключением gRPC

## Ошибка: "No connection established" / "Service unavailable"

### Шаг 1: Проверьте, что сервис запущен

```powershell
netstat -ano | findstr :8093

```
Должен быть вывод с `LISTENING`. Если нет - запустите сервис.

### Шаг 2: Убедитесь, что сервис перезапущен после изменений
**ВАЖНО:** После любых изменений в коде нужно перезапустить сервис!

1. Остановите текущий процесс

2. Пересоберите: `dotnet build`

3. Запустите заново

### Шаг 3: Проверьте настройки Postman

#### Обязательные настройки:

1. **URL должен быть HTTP (не HTTPS):**
   ```
   http://localhost:8093
   ```

2. **Отключите TLS:**

   - Вкладка **"Settings"** (внизу справа)

   - Найдите **"TLS"** или **"Use TLS"**

   - **Снимите галочку** или выберите **"Plain text"**

   - **НЕ используйте "TLS" или "HTTPS"**

3. **Импортируйте proto файл:**

   - Выберите **"Import a .proto file"**

   - Файл: `src/Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost/Protos/customers.proto`

### Шаг 4: Проверьте конфигурацию сервиса
Убедитесь, что в `Program.cs` настроен HTTP/2:

```csharp
webBuilder.UseKestrel(options =>
{
    options.ConfigureEndpointDefaults(listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
});

```
И что в `Startup.cs` отключен HTTPS редирект:

```csharp
// app.UseHttpsRedirection(); // Закомментировано

```

### Шаг 5: Альтернативные способы тестирования
Если Postman не работает, попробуйте:

#### Вариант 1: BloomRPC

1. Скачайте: https://github.com/bloomrpc/bloomrpc/releases

2. Импортируйте proto файл

3. URL: `localhost:8093`

4. Отключите TLS

#### Вариант 2: grpcurl

```powershell

# Установка
choco install grpcurl

# Проверка доступности
grpcurl -plaintext localhost:8093 list

# Вызов метода
grpcurl -plaintext -d '{}' localhost:8093 customers.CustomersService/GetCustomers

```

#### Вариант 3: C# тестовый клиент
См. файл `test-grpc-client.cs`

### Шаг 6: Проверьте логи сервиса
Посмотрите в консоль, где запущен сервис, на наличие ошибок при попытке подключения.

### Шаг 7: Проверьте, что Preferences сервис запущен
gRPC сервис может зависеть от Preferences API:

```powershell
netstat -ano | findstr :8094

```

### Типичные ошибки и решения

| Ошибка | Решение |

|--------|---------|

| "No connection established" | 1. Проверьте, что сервис запущен<br>2. Отключите TLS в Postman<br>3. Используйте `http://` (не `https://`) |

| "TLS required" | Отключите TLS в настройках Postman |

| "Service unavailable" | Перезапустите сервис после изменений в коде |

| "Connection refused" | Сервис не запущен или не слушает порт 8093 |

### Быстрая проверка
Выполните команду для проверки всех компонентов:

```powershell
.\quick-test-grpc.ps1

```

### Если ничего не помогает

1. Перезапустите компьютер (иногда помогает с сетевыми проблемами)

2. Проверьте брандмауэр Windows

3. Попробуйте другой порт (измените в `Program.cs` и перезапустите)

4. Используйте другой инструмент (BloomRPC, grpcurl)
