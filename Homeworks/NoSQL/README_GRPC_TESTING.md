# Тестирование gRPC сервиса Customers

## Способы тестирования gRPC сервиса

### Способ 1: Использование grpcurl (рекомендуется)

**Установка grpcurl:**
- Windows (Chocolatey): `choco install grpcurl`
- Или скачайте: https://github.com/fullstorydev/grpcurl/releases
- Добавьте в PATH

**Запуск тестов:**
```powershell
.\test-grpc-customers.ps1
```

**Ручное тестирование через grpcurl:**

1. **Проверить доступные сервисы:**
```bash
grpcurl -plaintext localhost:8093 list
```

2. **Получить список клиентов:**
```bash
grpcurl -plaintext -d '{}' localhost:8093 customers.CustomersService/GetCustomers
```

3. **Создать клиента:**
```bash
grpcurl -plaintext -d '{"first_name":"Иван","last_name":"Тестовый","email":"test@example.com","preference_ids":[]}' localhost:8093 customers.CustomersService/CreateCustomer
```

4. **Получить клиента по ID:**
```bash
grpcurl -plaintext -d '{"customer_id":"ВАШ_ID_КЛИЕНТА"}' localhost:8093 customers.CustomersService/GetCustomer
```

5. **Обновить клиента:**
```bash
grpcurl -plaintext -d '{"customer_id":"ВАШ_ID","first_name":"Иван","last_name":"Обновленный","email":"updated@example.com","preference_ids":[]}' localhost:8093 customers.CustomersService/UpdateCustomer
```

6. **Удалить клиента:**
```bash
grpcurl -plaintext -d '{"customer_id":"ВАШ_ID"}' localhost:8093 customers.CustomersService/DeleteCustomer
```

### Способ 2: Использование Postman

1. Откройте Postman
2. Создайте новый gRPC запрос
3. Импортируйте proto файл: `src/Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost/Protos/customers.proto`
4. Укажите URL: `http://localhost:8093`
5. Выберите метод и заполните параметры
6. Нажмите "Invoke"

### Способ 3: Создание тестового C# клиента

Создайте новый консольный проект:

```bash
dotnet new console -n GrpcTestClient
cd GrpcTestClient
```

Добавьте ссылки:
```bash
dotnet add package Grpc.Net.Client
dotnet add reference ../src/Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost/Pcf.GivingToCustomer.WebHost.csproj
```

Скопируйте код из `test-grpc-client.cs` в `Program.cs` и запустите:
```bash
dotnet run
```

### Способ 4: Использование BloomRPC (GUI клиент)

1. Скачайте BloomRPC: https://github.com/bloomrpc/bloomrpc/releases
2. Установите и запустите
3. Импортируйте proto файл: `src/Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost/Protos/customers.proto`
4. Укажите адрес: `localhost:8093`
5. Выберите метод и вызовите

### Способ 5: Написание интеграционных тестов

Создайте тесты в проекте `Pcf.GivingToCustomer.IntegrationTests`:

```csharp
[Fact]
public async Task GetCustomers_ShouldReturnCustomers()
{
    // Arrange
    using var channel = GrpcChannel.ForAddress("http://localhost:8093");
    var client = new CustomersService.CustomersServiceClient(channel);

    // Act
    var response = await client.GetCustomersAsync(new Empty());

    // Assert
    Assert.NotNull(response);
}
```

## Проверка доступности сервиса

Перед тестированием убедитесь, что:
1. Сервис `Pcf.GivingToCustomer.WebHost` запущен на порту 8093
2. Сервис `Pcf.Preferences.WebHost` запущен на порту 8094 (требуется для работы с предпочтениями)

Проверка:
```powershell
netstat -ano | findstr :8093
netstat -ano | findstr :8094
```

## Доступные методы gRPC

- `GetCustomers` - Получить список всех клиентов
- `GetCustomer` - Получить клиента по ID
- `CreateCustomer` - Создать нового клиента
- `UpdateCustomer` - Обновить клиента
- `DeleteCustomer` - Удалить клиента

## Полезные ссылки

- Документация gRPC для .NET: https://learn.microsoft.com/ru-ru/aspnet/core/grpc/
- grpcurl документация: https://github.com/fullstorydev/grpcurl
- Postman gRPC: https://www.postman.com/product/grpc-client/

