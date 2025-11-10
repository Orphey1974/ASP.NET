# Тестирование API микросервиса предпочтений
Clear-Host
Write-Host "Тестирование API микросервиса предпочтений..." -ForegroundColor Green

# Ждем запуска сервиса
Write-Host "Ожидание запуска сервиса..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Тест 1: Получение всех предпочтений
Write-Host "`n1. Получение всех предпочтений:" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "http://localhost:8094/api/v1/preferences" -Method GET
    $response | ConvertTo-Json -Depth 3
} catch {
    Write-Host "Ошибка: $($_.Exception.Message)" -ForegroundColor Red
}

# Тест 2: Получение предпочтения по ID
Write-Host "`n2. Получение предпочтения по ID:" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "http://localhost:8094/api/v1/preferences/11111111-1111-1111-1111-111111111111" -Method GET
    $response | ConvertTo-Json -Depth 3
} catch {
    Write-Host "Ошибка: $($_.Exception.Message)" -ForegroundColor Red
}

# Тест 3: Создание нового предпочтения
Write-Host "`n3. Создание нового предпочтения:" -ForegroundColor Cyan
try {
    $newPreference = @{
        Name = "Тестовое предпочтение"
        Description = "Описание тестового предпочтения"
    } | ConvertTo-Json

    $response = Invoke-RestMethod -Uri "http://localhost:8094/api/v1/preferences" -Method POST -Body $newPreference -ContentType "application/json"
    $response | ConvertTo-Json -Depth 3
} catch {
    Write-Host "Ошибка: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`nТестирование завершено!" -ForegroundColor Green
