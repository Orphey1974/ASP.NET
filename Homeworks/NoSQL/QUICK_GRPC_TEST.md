# Быстрое тестирование gRPC - Простые способы

## ✅ Рекомендуется: Использовать Postman (уже установлен)
Postman уже установлен и готов к работе. Это самый простой способ!

### Шаги:

1. **Откройте Postman**

2. **Создайте новый gRPC запрос:**

   - New → gRPC Request

3. **URL:**
   ```
   http://localhost:8093
   ```
   ⚠️ **ВАЖНО:** Используйте `http://` (не `https://`)

4. **ОТКЛЮЧИТЕ TLS:**

   - Вкладка **"Settings"** (внизу справа)

   - Найдите **"TLS"** или **"Use TLS"**

   - **СНИМИТЕ ГАЛОЧКУ** или выберите **"Plain text"**

5. **Импортируйте proto файл:**

   - Нажмите **"Import a .proto file"**

   - Выберите: `src\Pcf.GivingToCustomer\Pcf.GivingToCustomer.WebHost\Protos\customers.proto`

6. **Выберите метод:**

   - Сервис: `CustomersService`

   - Метод: `GetCustomers` (или любой другой)

7. **Нажмите "Invoke"**

## Альтернатива 1: BloomRPC (GUI клиент)

1. **Скачайте:** https://github.com/bloomrpc/bloomrpc/releases

2. **Установите** (просто распакуйте архив)

3. **Запустите BloomRPC**

4. **Импортируйте proto файл:**

   - Нажмите "+" → "Import Proto"

   - Выберите: `src\Pcf.GivingToCustomer\Pcf.GivingToCustomer.WebHost\Protos\customers.proto`

5. **URL:** `localhost:8093`

6. **Отключите TLS** (если есть опция)

7. **Выберите метод и вызовите**

## Альтернатива 2: Установить grpcurl вручную
Если все-таки нужен grpcurl:

### Шаг 1: Скачайте

1. Перейдите: https://github.com/fullstorydev/grpcurl/releases

2. Скачайте: `grpcurl_1.8.9_windows_x86_64.zip` (или последняя версия)

### Шаг 2: Распакуйте
Распакуйте архив в любую папку, например: `C:\Tools\grpcurl`

### Шаг 3: Используйте напрямую

```powershell

# Перейдите в папку с grpcurl.exe
cd C:\Tools\grpcurl

# Проверка доступности
.\grpcurl.exe -plaintext localhost:8093 list

# Получить список клиентов
.\grpcurl.exe -plaintext -d '{}' localhost:8093 customers.CustomersService/GetCustomers

```

### Шаг 4: (Опционально) Добавить в PATH
Если хотите использовать `grpcurl` из любой папки:

1. Скопируйте `grpcurl.exe` в `C:\Windows\System32` (требуются права администратора)

2. Или добавьте папку в PATH:

   - `Win + R` → `sysdm.cpl` → Enter

   - "Дополнительно" → "Переменные среды"

   - В "Системные переменные" найдите `Path` → "Изменить"

   - "Создать" → добавьте путь к папке с grpcurl.exe

   - "ОК" → перезапустите терминал

## Альтернатива 3: C# тестовый клиент
См. файл `test-grpc-client.cs` - пример кода для создания простого консольного приложения.

## Резюме
**Самый простой способ:** Используйте Postman - он уже установлен, просто отключите TLS!
**Если нужен командный инструмент:** Скачайте grpcurl вручную с GitHub.
**Если нужен GUI:** Используйте BloomRPC.
