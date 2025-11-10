# Ручная установка grpcurl на Windows

## Способ 1: Прямая ссылка (рекомендуется)

1. **Откройте браузер и перейдите:**
   ```
   https://github.com/fullstorydev/grpcurl/releases/latest
   ```

2. **Скачайте файл для Windows 64-bit:**
   - Найдите файл с названием типа: `grpcurl_1.8.9_windows_x86_64.zip`
   - Или просто нажмите на первый `.zip` файл для Windows

3. **Распакуйте архив:**
   - Распакуйте в любую папку, например: `C:\Tools\grpcurl`
   - Или в: `%USERPROFILE%\Tools\grpcurl`

4. **Используйте напрямую:**
   ```powershell
   # Перейдите в папку с grpcurl.exe
   cd C:\Tools\grpcurl

   # Проверка
   .\grpcurl.exe --version

   # Использование
   .\grpcurl.exe -plaintext localhost:8093 list
   ```

## Способ 2: Добавить в PATH (для использования из любой папки)

### Вариант A: Добавить папку в PATH (рекомендуется)

1. **Скопируйте `grpcurl.exe` в папку:**
   - Создайте папку: `C:\Tools\grpcurl`
   - Скопируйте туда `grpcurl.exe`

2. **Добавьте в PATH:**
   - Нажмите `Win + R`
   - Введите: `sysdm.cpl` → Enter
   - Вкладка **"Дополнительно"** → **"Переменные среды"**
   - В разделе **"Системные переменные"** найдите `Path`
   - Нажмите **"Изменить"**
   - Нажмите **"Создать"**
   - Введите: `C:\Tools\grpcurl`
   - Нажмите **"ОК"** во всех окнах
   - **Перезапустите терминал**

3. **Проверка:**
   ```powershell
   grpcurl --version
   ```

### Вариант B: Скопировать в System32 (требуются права администратора)

1. **Откройте PowerShell от имени администратора**
2. **Скопируйте файл:**
   ```powershell
   Copy-Item "C:\путь\к\grpcurl.exe" -Destination "C:\Windows\System32\grpcurl.exe"
   ```
3. **Проверка:**
   ```powershell
   grpcurl --version
   ```

## Быстрая ссылка для скачивания

Если GitHub открывается медленно, попробуйте прямую ссылку:
```
https://github.com/fullstorydev/grpcurl/releases/download/v1.8.9/grpcurl_1.8.9_windows_x86_64.zip
```

(Замените `v1.8.9` на актуальную версию, если нужно)

## Использование после установки

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

## Альтернатива: Использовать Postman

Если установка grpcurl вызывает проблемы, используйте Postman:
1. URL: `http://localhost:8093`
2. Отключите TLS в настройках
3. Импортируйте proto файл

Это проще и не требует установки дополнительных инструментов!


