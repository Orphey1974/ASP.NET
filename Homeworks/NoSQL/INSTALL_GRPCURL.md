# Установка grpcurl для Windows

## Способ 1: Скачать готовый бинарник (рекомендуется)

1. Перейдите на страницу релизов: https://github.com/fullstorydev/grpcurl/releases

2. Скачайте файл для Windows:

   - Для 64-bit: `grpcurl_1.8.9_windows_x86_64.zip` (или последняя версия)

   - Для 32-bit: `grpcurl_1.8.9_windows_x86_32.zip`

3. Распакуйте архив

4. Скопируйте `grpcurl.exe` в папку, которая есть в PATH, например:

   - `C:\Windows\System32` (требуются права администратора)

   - Или создайте папку `C:\Tools` и добавьте её в PATH

### Добавление в PATH (если нужно):

1. Нажмите `Win + R`, введите `sysdm.cpl`, нажмите Enter

2. Вкладка "Дополнительно" → "Переменные среды"

3. В "Системные переменные" найдите `Path` → "Изменить"

4. Добавьте путь к папке с `grpcurl.exe` (например, `C:\Tools`)

5. Нажмите "ОК" во всех окнах

6. Перезапустите терминал

## Способ 2: Использовать через Scoop (если установлен)

```powershell
scoop install grpcurl

```

## Способ 3: Использовать через Go (если установлен Go)

```powershell
go install github.com/fullstorydev/grpcurl/cmd/grpcurl@latest

```

## Проверка установки
После установки проверьте:

```powershell
grpcurl --version

```
Должен вывести версию, например: `grpcurl 1.8.9`

## Использование grpcurl

### Проверка доступности сервиса:

```powershell
grpcurl -plaintext localhost:8093 list

```

### Получить список клиентов:

```powershell
grpcurl -plaintext -d '{}' localhost:8093 customers.CustomersService/GetCustomers

```

### Создать клиента:

```powershell
grpcurl -plaintext -d '{"first_name":"Иван","last_name":"Тестовый","email":"test@example.com","preference_ids":[]}' localhost:8093 customers.CustomersService/CreateCustomer

```

## Альтернатива: Использовать готовый скрипт
Я создал скрипт `test-grpc-customers.ps1`, который автоматически проверит наличие grpcurl и выполнит тесты.

## Если не хотите устанавливать grpcurl
Используйте другие инструменты:

1. **Postman** - уже установлен, просто отключите TLS

2. **BloomRPC** - https://github.com/bloomrpc/bloomrpc/releases

3. **C# тестовый клиент** - см. `test-grpc-client.cs`
