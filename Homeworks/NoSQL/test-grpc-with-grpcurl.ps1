# Скрипт для тестирования gRPC сервиса через grpcurl
# Использование: .\test-grpc-with-grpcurl.ps1

Clear-Host
$grpcUrl = "localhost:8093"

Write-Host "=== Тестирование gRPC сервиса через grpcurl ===" -ForegroundColor Green
Write-Host ""

# Проверка наличия grpcurl
$grpcurlCheck = Get-Command grpcurl -ErrorAction SilentlyContinue
if (-not $grpcurlCheck) {
    Write-Host "ОШИБКА: grpcurl не найден в PATH!" -ForegroundColor Red
    Write-Host "Убедитесь, что grpcurl установлен и добавлен в PATH." -ForegroundColor Yellow
    exit 1
}

Write-Host "✓ grpcurl найден: $(grpcurl --version)" -ForegroundColor Green
Write-Host ""

# Проверка доступности сервиса
Write-Host "1. Проверка доступности сервиса на $grpcUrl..." -ForegroundColor Yellow
try {
    $services = grpcurl -plaintext $grpcUrl list 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ✓ Сервис доступен" -ForegroundColor Green
        Write-Host "   Доступные сервисы:" -ForegroundColor Cyan
        $services | ForEach-Object { Write-Host "     - $_" -ForegroundColor Gray }
    } else {
        Write-Host "   ✗ Сервис недоступен" -ForegroundColor Red
        Write-Host "   Убедитесь, что сервис запущен на порту 8093" -ForegroundColor Yellow
        exit 1
    }
} catch {
    Write-Host "   ✗ Ошибка подключения: $_" -ForegroundColor Red
    Write-Host "   Убедитесь, что сервис запущен" -ForegroundColor Yellow
    exit 1
}

Write-Host ""

# Тест 1: Получить список клиентов
Write-Host "2. Тест: GetCustomers (получить список клиентов)..." -ForegroundColor Yellow
try {
    $response = grpcurl -plaintext -d '{}' $grpcUrl customers.CustomersService/GetCustomers 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ✓ Успешно" -ForegroundColor Green
        Write-Host "   Ответ:" -ForegroundColor Cyan
        $response | ForEach-Object { Write-Host "     $_" -ForegroundColor Gray }
    } else {
        Write-Host "   ✗ Ошибка" -ForegroundColor Red
        Write-Host $response -ForegroundColor Red
    }
} catch {
    Write-Host "   ✗ Ошибка: $_" -ForegroundColor Red
}

Write-Host ""

# Тест 2: Создать клиента
Write-Host "3. Тест: CreateCustomer (создать клиента)..." -ForegroundColor Yellow
$createRequest = @{
    first_name = "Иван"
    last_name = "Тестовый"
    email = "ivan.test@example.com"
    preference_ids = @()
} | ConvertTo-Json -Compress

try {
    $response = grpcurl -plaintext -d $createRequest $grpcUrl customers.CustomersService/CreateCustomer 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ✓ Клиент создан" -ForegroundColor Green
        Write-Host "   Ответ:" -ForegroundColor Cyan
        $response | ForEach-Object { Write-Host "     $_" -ForegroundColor Gray }

        # Извлекаем ID созданного клиента
        if ($response -match 'id:\s*"([^"]+)"') {
            $customerId = $matches[1]
            Write-Host "   Созданный ID клиента: $customerId" -ForegroundColor Cyan

            # Тест 3: Получить созданного клиента
            Write-Host ""
            Write-Host "4. Тест: GetCustomer (получить клиента по ID)..." -ForegroundColor Yellow
            $getRequest = @{
                customer_id = $customerId
            } | ConvertTo-Json -Compress

            try {
                $getResponse = grpcurl -plaintext -d $getRequest $grpcUrl customers.CustomersService/GetCustomer 2>&1
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "   ✓ Клиент найден" -ForegroundColor Green
                    Write-Host "   Ответ:" -ForegroundColor Cyan
                    $getResponse | ForEach-Object { Write-Host "     $_" -ForegroundColor Gray }
                } else {
                    Write-Host "   ✗ Ошибка" -ForegroundColor Red
                }
            } catch {
                Write-Host "   ✗ Ошибка: $_" -ForegroundColor Red
            }
        }
    } else {
        Write-Host "   ✗ Ошибка создания" -ForegroundColor Red
        Write-Host $response -ForegroundColor Red
    }
} catch {
    Write-Host "   ✗ Ошибка: $_" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== Тестирование завершено ===" -ForegroundColor Green
Write-Host ""
Write-Host "Полезные команды:" -ForegroundColor Cyan
Write-Host "  grpcurl -plaintext localhost:8093 list" -ForegroundColor Gray
Write-Host "  grpcurl -plaintext -d '{}' localhost:8093 customers.CustomersService/GetCustomers" -ForegroundColor Gray


