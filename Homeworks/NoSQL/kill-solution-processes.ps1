# Script to kill processes using solution ports
# Used before starting services to prevent port conflicts

param(
    [switch]$Verbose
)

$ErrorActionPreference = "Continue"

# Ports used by solution services
$ports = @(
    8091,  # Administration
    8092,  # ReceivingFromPartner
    8093,  # GivingToCustomer (HTTPS)
    8094,  # Preferences
    8095   # GivingToCustomer (HTTP)
)

# Process names to kill
$processNames = @(
    "Pcf.Administration.WebHost",
    "Pcf.GivingToCustomer.WebHost",
    "Pcf.ReceivingFromPartner.WebHost",
    "Pcf.Preferences.WebHost"
)

Write-Host "=== Killing solution processes ===" -ForegroundColor Cyan
Write-Host ""

$killedCount = 0
$portKilledCount = 0

# Kill processes by name
Write-Host "Killing processes by name..." -ForegroundColor Yellow
foreach ($processName in $processNames) {
    $processes = Get-Process -Name $processName -ErrorAction SilentlyContinue
    if ($processes) {
        foreach ($process in $processes) {
            try {
                if ($Verbose) {
                    Write-Host "  Killing process: $($process.Name) (PID: $($process.Id))" -ForegroundColor Gray
                }
                Stop-Process -Id $process.Id -Force -ErrorAction Stop
                Write-Host "  OK Killed process: $($process.Name) (PID: $($process.Id))" -ForegroundColor Green
                $killedCount++
            }
            catch {
                Write-Host "  ERROR Failed to kill process $($process.Name) (PID: $($process.Id)): $_" -ForegroundColor Red
            }
        }
    }
}

# Kill processes by port
Write-Host ""
Write-Host "Killing processes by port..." -ForegroundColor Yellow
foreach ($port in $ports) {
    try {
        # Get information about processes using the port
        $netstatOutput = netstat -ano | Select-String ":$port\s+.*LISTENING"

        if ($netstatOutput) {
            foreach ($line in $netstatOutput) {
                # Extract PID from netstat line
                $pid = ($line -split '\s+')[-1]

                if ($pid -and $pid -match '^\d+$') {
                    try {
                        $process = Get-Process -Id $pid -ErrorAction SilentlyContinue
                        if ($process) {
                            if ($Verbose) {
                                Write-Host "  Killing process on port $port : $($process.Name) (PID: $pid)" -ForegroundColor Gray
                            }
                            Stop-Process -Id $pid -Force -ErrorAction Stop
                            Write-Host "  OK Killed process on port $port : $($process.Name) (PID: $pid)" -ForegroundColor Green
                            $portKilledCount++
                        }
                    }
                    catch {
                        Write-Host "  ERROR Failed to kill process on port $port (PID: $pid): $_" -ForegroundColor Red
                    }
                }
            }
        }
    }
    catch {
        Write-Host "  ERROR Failed to check port $port : $_" -ForegroundColor Red
    }
}

Write-Host ""
if ($killedCount -eq 0 -and $portKilledCount -eq 0) {
    Write-Host "No running processes to kill." -ForegroundColor Green
}
else {
    Write-Host "Killed processes: $killedCount (by name) + $portKilledCount (by port)" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "=== Done ===" -ForegroundColor Cyan

# Задержка для освобождения файлов и портов перед сборкой
if ($killedCount -gt 0 -or $portKilledCount -gt 0) {
    Write-Host "Waiting for resources to be released..." -ForegroundColor Gray
    Start-Sleep -Seconds 1
}
