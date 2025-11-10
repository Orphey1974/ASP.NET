# Скрипт для добавления grpcurl в PATH
# Использование: .\add-grpcurl-to-path.ps1

Clear-Host
$pathToAdd = "E:\Install\grpcurl_1.9.3_windows_x86_64"

Write-Host "=== Добавление grpcurl в PATH ===" -ForegroundColor Green
Write-Host ""

# Проверка существования папки
if (-not (Test-Path $pathToAdd)) {
    Write-Host "ОШИБКА: Папка не найдена: $pathToAdd" -ForegroundColor Red
    Write-Host "Проверьте путь и попробуйте снова." -ForegroundColor Yellow
    exit 1
}

# Проверка наличия grpcurl.exe
$grpcurlExe = Join-Path $pathToAdd "grpcurl.exe"
if (-not (Test-Path $grpcurlExe)) {
    Write-Host "ОШИБКА: grpcurl.exe не найден в папке: $pathToAdd" -ForegroundColor Red
    Write-Host "Убедитесь, что файл grpcurl.exe находится в указанной папке." -ForegroundColor Yellow
    exit 1
}

Write-Host "✓ Папка найдена: $pathToAdd" -ForegroundColor Green
Write-Host "✓ grpcurl.exe найден" -ForegroundColor Green
Write-Host ""

# Получение текущего PATH
$currentPath = [Environment]::GetEnvironmentVariable("Path", "User")

# Проверка, не добавлена ли уже папка
if ($currentPath -like "*$pathToAdd*") {
    Write-Host "Папка уже добавлена в PATH!" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Проверка доступности grpcurl:" -ForegroundColor Cyan
    try {
        $env:Path = [Environment]::GetEnvironmentVariable("Path", "User") + ";" + [Environment]::GetEnvironmentVariable("Path", "Machine")
        $version = & grpcurl --version 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ grpcurl доступен: $version" -ForegroundColor Green
        } else {
            Write-Host "⚠ grpcurl не доступен. Перезапустите терминал." -ForegroundColor Yellow
        }
    } catch {
        Write-Host "⚠ Перезапустите терминал для использования grpcurl" -ForegroundColor Yellow
    }
    exit 0
}

# Добавление в PATH
Write-Host "Добавление в PATH..." -ForegroundColor Yellow
try {
    if ([string]::IsNullOrEmpty($currentPath)) {
        $newPath = $pathToAdd
    } else {
        $newPath = $currentPath + ";$pathToAdd"
    }

    [Environment]::SetEnvironmentVariable("Path", $newPath, "User")
    Write-Host "✓ Папка успешно добавлена в PATH" -ForegroundColor Green
} catch {
    Write-Host "✗ Ошибка при добавлении в PATH: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Попробуйте добавить вручную:" -ForegroundColor Yellow
    Write-Host "1. Win + R → sysdm.cpl → Enter" -ForegroundColor Gray
    Write-Host "2. Дополнительно → Переменные среды" -ForegroundColor Gray
    Write-Host "3. В 'Переменные пользователя' найдите Path → Изменить" -ForegroundColor Gray
    Write-Host "4. Создать → $pathToAdd" -ForegroundColor Gray
    exit 1
}

Write-Host ""
Write-Host "=== Готово! ===" -ForegroundColor Green
Write-Host ""
Write-Host "ВАЖНО: Перезапустите терминал для применения изменений!" -ForegroundColor Yellow
Write-Host ""
Write-Host "После перезапуска проверьте:" -ForegroundColor Cyan
Write-Host "  grpcurl --version" -ForegroundColor Gray
Write-Host ""
Write-Host "Использование:" -ForegroundColor Cyan
Write-Host "  grpcurl -plaintext localhost:8093 list" -ForegroundColor Gray
Write-Host "  grpcurl -plaintext -d '{}' localhost:8093 customers.CustomersService/GetCustomers" -ForegroundColor Gray


