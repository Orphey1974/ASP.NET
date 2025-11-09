# Скрипт для остановки всех сервисов решения Pcf.Preferences
# Использование: .\stop-preferences-services.ps1

Clear-Host
Write-Host "🛑 Остановка сервисов решения Pcf.Preferences..." -ForegroundColor Yellow
Write-Host ""

# Порт, который использует сервис Preferences
$port = 8094

# Находим процессы, использующие порт 8094
Write-Host "Поиск процессов на порту $port..." -ForegroundColor Cyan
$connections = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue

if ($connections) {
    $processIds = $connections | Select-Object -ExpandProperty OwningProcess -Unique
    Write-Host "Найдено процессов: $($processIds.Count)" -ForegroundColor Cyan

    foreach ($processId in $processIds) {
        try {
            $process = Get-Process -Id $processId -ErrorAction Stop
            Write-Host "  Остановка процесса: $($process.ProcessName) (PID: $processId)" -ForegroundColor Yellow
            Stop-Process -Id $processId -Force -ErrorAction Stop
            Write-Host "  ✅ Процесс $processId остановлен" -ForegroundColor Green
        }
        catch {
            Write-Host "  ⚠️ Не удалось остановить процесс $processId : $($_.Exception.Message)" -ForegroundColor Red
        }
    }
}
else {
    Write-Host "Процессы на порту $port не найдены" -ForegroundColor Gray
}

# Находим процессы dotnet, связанные с Pcf.Preferences
Write-Host ""
Write-Host "Поиск процессов dotnet, связанных с Pcf.Preferences..." -ForegroundColor Cyan

# Ищем процессы по имени файла
$preferencesProcesses = Get-Process | Where-Object {
    $_.ProcessName -like "*Preferences*" -or
    ($_.ProcessName -eq "dotnet" -and $_.Path -like "*Pcf.Preferences*")
}

# Также ищем через WMI для более точного поиска по командной строке
try {
    $wmiProcesses = Get-WmiObject Win32_Process | Where-Object {
        $_.CommandLine -like "*Pcf.Preferences*" -or
        $_.CommandLine -like "*Preferences.WebHost*"
    } | ForEach-Object {
        Get-Process -Id $_.ProcessId -ErrorAction SilentlyContinue
    }

    if ($wmiProcesses) {
        $preferencesProcesses = @($preferencesProcesses) + @($wmiProcesses) | Select-Object -Unique -Property Id
    }
}
catch {
    Write-Host "  (WMI поиск недоступен, используется базовый поиск)" -ForegroundColor Gray
}

if ($preferencesProcesses) {
    Write-Host "Найдено процессов: $($preferencesProcesses.Count)" -ForegroundColor Cyan

    foreach ($process in $preferencesProcesses) {
        try {
            Write-Host "  Остановка процесса: $($process.ProcessName) (PID: $($process.Id))" -ForegroundColor Yellow
            Stop-Process -Id $process.Id -Force -ErrorAction Stop
            Write-Host "  ✅ Процесс $($process.Id) остановлен" -ForegroundColor Green
        }
        catch {
            Write-Host "  ⚠️ Не удалось остановить процесс $($process.Id) : $($_.Exception.Message)" -ForegroundColor Red
        }
    }
}
else {
    Write-Host "Процессы Pcf.Preferences не найдены" -ForegroundColor Gray
}

# Проверяем, что порт освобожден
Write-Host ""
Start-Sleep -Seconds 2
$check = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue

if ($check) {
    Write-Host "⚠️ Порт $port все еще занят" -ForegroundColor Red
    Write-Host "Попробуйте остановить процессы вручную или перезагрузить компьютер" -ForegroundColor Yellow
}
else {
    Write-Host "✅ Порт $port освобожден" -ForegroundColor Green
}

Write-Host ""
Write-Host "Готово! Все сервисы Pcf.Preferences остановлены." -ForegroundColor Green

