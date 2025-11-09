# Скрипт для установки grpcurl на Windows
# Использование: .\install-grpcurl.ps1

Clear-Host
Write-Host "=== Установка grpcurl ===" -ForegroundColor Green
Write-Host ""

# Проверка, не установлен ли уже grpcurl
$grpcurlCheck = Get-Command grpcurl -ErrorAction SilentlyContinue
if ($grpcurlCheck) {
    Write-Host "grpcurl уже установлен!" -ForegroundColor Green
    Write-Host "Версия: " -NoNewline
    grpcurl --version
    exit 0
}

# Получение информации о последней версии
Write-Host "1. Получение информации о последней версии..." -ForegroundColor Yellow
try {
    $latestRelease = Invoke-RestMethod -Uri "https://api.github.com/repos/fullstorydev/grpcurl/releases/latest"
    $version = $latestRelease.tag_name
    Write-Host "   Последняя версия: $version" -ForegroundColor Green

    # Поиск файла для Windows 64-bit
    $windows64Asset = $latestRelease.assets | Where-Object { $_.name -like "*windows_x86_64.zip" } | Select-Object -First 1

    if (-not $windows64Asset) {
        Write-Host "   ✗ Файл для Windows 64-bit не найден" -ForegroundColor Red
        exit 1
    }

    Write-Host "   Найден файл: $($windows64Asset.name)" -ForegroundColor Green
} catch {
    Write-Host "   ✗ Ошибка при получении информации: $_" -ForegroundColor Red
    exit 1
}

# Определение папки для установки
$installDir = "$env:USERPROFILE\Tools\grpcurl"
$downloadPath = "$env:TEMP\grpcurl.zip"

# Создание папки для установки
Write-Host ""
Write-Host "2. Создание папки для установки..." -ForegroundColor Yellow
if (-not (Test-Path $installDir)) {
    New-Item -ItemType Directory -Path $installDir -Force | Out-Null
    Write-Host "   Создана папка: $installDir" -ForegroundColor Green
} else {
    Write-Host "   Папка уже существует: $installDir" -ForegroundColor Gray
}

# Скачивание
Write-Host ""
Write-Host "3. Скачивание grpcurl..." -ForegroundColor Yellow
Write-Host "   URL: $($windows64Asset.browser_download_url)" -ForegroundColor Gray
try {
    Invoke-WebRequest -Uri $windows64Asset.browser_download_url -OutFile $downloadPath
    Write-Host "   ✓ Скачивание завершено" -ForegroundColor Green
} catch {
    Write-Host "   ✗ Ошибка при скачивании: $_" -ForegroundColor Red
    exit 1
}

# Распаковка
Write-Host ""
Write-Host "4. Распаковка..." -ForegroundColor Yellow
try {
    Expand-Archive -Path $downloadPath -DestinationPath $installDir -Force
    Write-Host "   ✓ Распаковка завершена" -ForegroundColor Green
} catch {
    Write-Host "   ✗ Ошибка при распаковке: $_" -ForegroundColor Red
    exit 1
}

# Поиск grpcurl.exe
$grpcurlExe = Get-ChildItem -Path $installDir -Filter "grpcurl.exe" -Recurse | Select-Object -First 1
if (-not $grpcurlExe) {
    Write-Host "   ✗ grpcurl.exe не найден в архиве" -ForegroundColor Red
    exit 1
}

# Перемещение в корень папки установки (если нужно)
if ($grpcurlExe.DirectoryName -ne $installDir) {
    Move-Item -Path $grpcurlExe.FullName -Destination $installDir -Force
    Write-Host "   ✓ Файл перемещен в: $installDir" -ForegroundColor Green
}

# Удаление временного файла
Remove-Item -Path $downloadPath -Force -ErrorAction SilentlyContinue

# Проверка установки
Write-Host ""
Write-Host "5. Проверка установки..." -ForegroundColor Yellow
$grpcurlPath = Join-Path $installDir "grpcurl.exe"
if (Test-Path $grpcurlPath) {
    Write-Host "   ✓ grpcurl.exe найден: $grpcurlPath" -ForegroundColor Green

    # Попытка запуска для проверки
    try {
        $version = & $grpcurlPath --version 2>&1
        Write-Host "   Версия: $version" -ForegroundColor Green
    } catch {
        Write-Host "   ⚠ Не удалось получить версию, но файл существует" -ForegroundColor Yellow
    }
} else {
    Write-Host "   ✗ grpcurl.exe не найден" -ForegroundColor Red
    exit 1
}

# Добавление в PATH (опционально)
Write-Host ""
Write-Host "6. Добавление в PATH..." -ForegroundColor Yellow
$currentPath = [Environment]::GetEnvironmentVariable("Path", "User")
if ($currentPath -notlike "*$installDir*") {
    $response = Read-Host "   Добавить $installDir в PATH? (y/n)"
    if ($response -eq "y" -or $response -eq "Y") {
        $newPath = $currentPath + ";$installDir"
        [Environment]::SetEnvironmentVariable("Path", $newPath, "User")
        Write-Host "   ✓ Добавлено в PATH" -ForegroundColor Green
        Write-Host "   ⚠ Перезапустите терминал для применения изменений" -ForegroundColor Yellow
    } else {
        Write-Host "   Пропущено. Используйте полный путь: $grpcurlPath" -ForegroundColor Gray
    }
} else {
    Write-Host "   ✓ Уже в PATH" -ForegroundColor Green
}

Write-Host ""
Write-Host "=== Установка завершена! ===" -ForegroundColor Green
Write-Host ""
Write-Host "Использование:" -ForegroundColor Cyan
Write-Host "  $grpcurlPath -plaintext localhost:8093 list" -ForegroundColor Gray
Write-Host ""
Write-Host "Или если добавлено в PATH (после перезапуска терминала):" -ForegroundColor Cyan
Write-Host "  grpcurl -plaintext localhost:8093 list" -ForegroundColor Gray


