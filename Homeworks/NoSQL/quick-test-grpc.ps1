# Быстрая проверка доступности gRPC сервиса
# Этот скрипт проверяет, что сервис запущен и доступен

Clear-Host
Write-Host "=== Проверка gRPC сервиса Customers ===" -ForegroundColor Green
Write-Host ""

# Проверка доступности порта
Write-Host "1. Проверка доступности порта 8093..." -ForegroundColor Yellow
$portCheck = netstat -ano | findstr :8093
if ($portCheck) {
    Write-Host "   ✓ Порт 8093 слушается" -ForegroundColor Green
    $portCheck | ForEach-Object { Write-Host "     $_" -ForegroundColor Gray }
} else {
    Write-Host "   ✗ Порт 8093 не слушается. Запустите сервис Pcf.GivingToCustomer.WebHost" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Проверка доступности HTTP endpoint (gRPC работает поверх HTTP/2)
Write-Host "2. Проверка HTTP endpoint..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:8093" -UseBasicParsing -TimeoutSec 3 -ErrorAction Stop
    Write-Host "   ✓ HTTP endpoint доступен (статус: $($response.StatusCode))" -ForegroundColor Green
} catch {
    Write-Host "   ⚠ HTTP endpoint недоступен (это нормально для gRPC)" -ForegroundColor Yellow
}

Write-Host ""

# Информация о тестировании
Write-Host "=== Инструкции по тестированию ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "Сервис gRPC запущен и готов к тестированию!" -ForegroundColor Green
Write-Host ""
Write-Host "Доступные способы тестирования:" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Postman (рекомендуется):" -ForegroundColor Cyan
Write-Host "   - Откройте Postman" -ForegroundColor White
Write-Host "   - Создайте новый gRPC запрос" -ForegroundColor White
Write-Host "   - Импортируйте: src\Pcf.GivingToCustomer\Pcf.GivingToCustomer.WebHost\Protos\customers.proto" -ForegroundColor White
Write-Host "   - URL: localhost:8093" -ForegroundColor White
Write-Host ""
Write-Host "2. BloomRPC:" -ForegroundColor Cyan
Write-Host "   - Скачайте: https://github.com/bloomrpc/bloomrpc/releases" -ForegroundColor White
Write-Host "   - Импортируйте proto файл и укажите localhost:8093" -ForegroundColor White
Write-Host ""
Write-Host "3. grpcurl (командная строка):" -ForegroundColor Cyan
Write-Host "   - Установите: choco install grpcurl" -ForegroundColor White
Write-Host "   - Запустите: .\test-grpc-customers.ps1" -ForegroundColor White
Write-Host ""
Write-Host "4. C# тестовый клиент:" -ForegroundColor Cyan
Write-Host "   - См. файл: test-grpc-client.cs" -ForegroundColor White
Write-Host "   - Создайте консольный проект и добавьте ссылки на WebHost проект" -ForegroundColor White
Write-Host ""
Write-Host "Доступные методы gRPC:" -ForegroundColor Yellow
Write-Host "  - GetCustomers" -ForegroundColor White
Write-Host "  - GetCustomer" -ForegroundColor White
Write-Host "  - CreateCustomer" -ForegroundColor White
Write-Host "  - UpdateCustomer" -ForegroundColor White
Write-Host "  - DeleteCustomer" -ForegroundColor White
Write-Host ""
Write-Host "Подробнее: см. README_GRPC_TESTING.md" -ForegroundColor Cyan

