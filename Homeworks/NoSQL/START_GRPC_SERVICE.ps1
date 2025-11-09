# Скрипт для запуска gRPC сервиса GivingToCustomer
# Использование: .\START_GRPC_SERVICE.ps1

Clear-Host
Write-Host "=== Запуск gRPC сервиса GivingToCustomer ===" -ForegroundColor Green
Write-Host ""

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
Write-Host "2. Запуск сервиса на порту 8093..." -ForegroundColor Yellow
Write-Host "   Сервис будет доступен по адресу: http://localhost:8093" -ForegroundColor Cyan
Write-Host "   Swagger UI: http://localhost:8093/swagger" -ForegroundColor Cyan
Write-Host "   gRPC endpoint: http://localhost:8093" -ForegroundColor Cyan
Write-Host ""
Write-Host "   Для остановки нажмите Ctrl+C" -ForegroundColor Gray
Write-Host ""

# Запуск сервиса
try {
    Set-Location $projectPath
    dotnet run
} catch {
    Write-Host "Ошибка при запуске: $_" -ForegroundColor Red
    exit 1
} finally {
    Set-Location $PSScriptRoot
}

