# Скрипт для запуска gRPC сервиса GivingToCustomer
# Использование: .\START_GRPC_SERVICE.ps1

Clear-Host
Write-Host "=== Запуск gRPC сервиса GivingToCustomer ===" -ForegroundColor Green
Write-Host ""

# Сохраняем исходную директорию
$originalLocation = Get-Location

# Определяем корень решения (где находится скрипт)
$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptRoot

$projectPath = "src/Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost"

# Проверка существования проекта
if (-not (Test-Path $projectPath)) {
    Write-Host "ОШИБКА: Проект не найден по пути: $projectPath" -ForegroundColor Red
    exit 1
}

# Проверка, не запущен ли уже сервис
$portCheck = netstat -ano | findstr :8093
if ($portCheck) {
    Write-Host "⚠ ВНИМАНИЕ: Порт 8093 уже занят!" -ForegroundColor Yellow
    Write-Host "   Занятые соединения:" -ForegroundColor Gray
    $portCheck | ForEach-Object { Write-Host "   $_" -ForegroundColor Gray }
    Write-Host ""
    $response = Read-Host "   Продолжить запуск? (y/n)"
    if ($response -ne "y") {
        Write-Host "Запуск отменен." -ForegroundColor Yellow
        exit 0
    }
}

Write-Host "1. Сборка проекта..." -ForegroundColor Yellow
try {
    dotnet build $projectPath --no-restore
    if ($LASTEXITCODE -ne 0) {
        Write-Host "   ✗ Ошибка сборки" -ForegroundColor Red
        exit 1
    }
    Write-Host "   ✓ Сборка успешна" -ForegroundColor Green
} catch {
    Write-Host "   ✗ Ошибка: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "2. Запуск сервиса на портах 8093 (HTTPS) и 8094 (HTTP)..." -ForegroundColor Yellow
Write-Host "   gRPC endpoint (HTTPS/HTTP/2): https://localhost:8093" -ForegroundColor Cyan
Write-Host "   HTTP endpoint (для обратной совместимости): http://localhost:8094" -ForegroundColor Cyan
Write-Host "   Swagger UI (REST API): https://localhost:8093/swagger" -ForegroundColor Cyan
Write-Host "   Примечание: gRPC использует HTTP/2 с TLS (h2) - рекомендуется для grpcurl" -ForegroundColor Gray
Write-Host ""
Write-Host "   Для остановки нажмите Ctrl+C" -ForegroundColor Gray
Write-Host ""

# Запуск сервиса
try {
    $fullProjectPath = Join-Path $scriptRoot $projectPath
    Set-Location $fullProjectPath
    dotnet run
} catch {
    Write-Host "Ошибка при запуске: $_" -ForegroundColor Red
    exit 1
} finally {
    Set-Location $originalLocation
}