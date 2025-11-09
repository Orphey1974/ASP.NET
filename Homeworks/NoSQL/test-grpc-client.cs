// Простой тестовый клиент для gRPC сервиса Customers
// Использование: dotnet run --project test-grpc-client.csproj
// Или скомпилируйте и запустите: csc test-grpc-client.cs

using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Pcf.GivingToCustomer.WebHost.Protos;
using Google.Protobuf.WellKnownTypes;

namespace GrpcTestClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Тестирование gRPC сервиса Customers ===\n");

            // Создаем канал для подключения к gRPC сервису
            using var channel = GrpcChannel.ForAddress("http://localhost:8093");
            var client = new CustomersService.CustomersServiceClient(channel);

            try
            {
                // Тест 1: Получить список всех клиентов
                Console.WriteLine("1. Тест: GetCustomers (получить список клиентов)...");
                var getCustomersResponse = await client.GetCustomersAsync(new Empty());
                Console.WriteLine($"   ✓ Успешно. Найдено клиентов: {getCustomersResponse.Customers.Count}");
                foreach (var customer in getCustomersResponse.Customers)
                {
                    Console.WriteLine($"     - {customer.FirstName} {customer.LastName} ({customer.Email})");
                }
                Console.WriteLine();

                // Тест 2: Создать нового клиента
                Console.WriteLine("2. Тест: CreateCustomer (создать клиента)...");
                var createRequest = new CreateCustomerRequest
                {
                    FirstName = "Иван",
                    LastName = "Тестовый",
                    Email = "ivan.test@example.com"
                };

                var createResponse = await client.CreateCustomerAsync(createRequest);
                Console.WriteLine($"   ✓ Клиент создан");
                Console.WriteLine($"     ID: {createResponse.Id}");
                Console.WriteLine($"     Имя: {createResponse.FirstName} {createResponse.LastName}");
                Console.WriteLine($"     Email: {createResponse.Email}");
                Console.WriteLine();

                // Тест 3: Получить созданного клиента по ID
                if (!string.IsNullOrEmpty(createResponse.Id))
                {
                    Console.WriteLine("3. Тест: GetCustomer (получить клиента по ID)...");
                    var getCustomerRequest = new GetCustomerRequest
                    {
                        CustomerId = createResponse.Id
                    };

                    var getCustomerResponse = await client.GetCustomerAsync(getCustomerRequest);
                    Console.WriteLine($"   ✓ Клиент найден");
                    Console.WriteLine($"     ID: {getCustomerResponse.Id}");
                    Console.WriteLine($"     Имя: {getCustomerResponse.FirstName} {getCustomerResponse.LastName}");
                    Console.WriteLine($"     Email: {getCustomerResponse.Email}");
                    Console.WriteLine($"     Предпочтений: {getCustomerResponse.Preferences.Count}");
                    Console.WriteLine($"     Промокодов: {getCustomerResponse.PromoCodes.Count}");
                    Console.WriteLine();
                }

                // Тест 4: Обновить клиента
                if (!string.IsNullOrEmpty(createResponse.Id))
                {
                    Console.WriteLine("4. Тест: UpdateCustomer (обновить клиента)...");
                    var updateRequest = new UpdateCustomerRequest
                    {
                        CustomerId = createResponse.Id,
                        FirstName = "Иван",
                        LastName = "Обновленный",
                        Email = "ivan.updated@example.com"
                    };

                    await client.UpdateCustomerAsync(updateRequest);
                    Console.WriteLine($"   ✓ Клиент обновлен");
                    Console.WriteLine();
                }

                // Тест 5: Удалить клиента
                if (!string.IsNullOrEmpty(createResponse.Id))
                {
                    Console.WriteLine("5. Тест: DeleteCustomer (удалить клиента)...");
                    var deleteRequest = new DeleteCustomerRequest
                    {
                        CustomerId = createResponse.Id
                    };

                    await client.DeleteCustomerAsync(deleteRequest);
                    Console.WriteLine($"   ✓ Клиент удален");
                    Console.WriteLine();
                }

                Console.WriteLine("=== Все тесты пройдены успешно! ===");
            }
            catch (Grpc.Core.RpcException ex)
            {
                Console.WriteLine($"✗ Ошибка gRPC: {ex.Status.StatusCode} - {ex.Status.Detail}");
                Console.WriteLine($"  Сообщение: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Ошибка: {ex.Message}");
                Console.WriteLine($"  StackTrace: {ex.StackTrace}");
            }
        }
    }
}

