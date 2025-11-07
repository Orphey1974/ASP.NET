# Script to kill processes locking files in bin folder
param(
    [string]$ProjectDirectory = $PSScriptRoot
)

Write-Host "Checking for processes locking files in bin folder..." -ForegroundColor Cyan

# Get solution root directory
$solutionRoot = if (Test-Path (Join-Path $ProjectDirectory "..\..\src")) {
    Split-Path (Split-Path $ProjectDirectory -Parent) -Parent
} elseif (Test-Path (Join-Path $ProjectDirectory "..\src")) {
    Split-Path $ProjectDirectory -Parent
} else {
    $ProjectDirectory
}

# Find all bin folders in solution
$binPaths = @()
Get-ChildItem -Path $solutionRoot -Recurse -Directory -Filter "bin" -ErrorAction SilentlyContinue | ForEach-Object {
    $binPaths += $_.FullName
}

if ($binPaths.Count -eq 0) {
    Write-Host "No bin folders found" -ForegroundColor Yellow
    exit 0
}

Write-Host "Found bin folders: $($binPaths.Count)" -ForegroundColor Green

# Find locked files
$lockedFiles = @()
foreach ($binPath in $binPaths) {
    Get-ChildItem -Path $binPath -Recurse -File -ErrorAction SilentlyContinue | ForEach-Object {
        try {
            $fileStream = [System.IO.File]::OpenWrite($_.FullName)
            $fileStream.Close()
        }
        catch {
            $lockedFiles += $_
        }
    }
}

if ($lockedFiles.Count -eq 0) {
    Write-Host "No locked files found" -ForegroundColor Green
    exit 0
}

Write-Host "Found locked files: $($lockedFiles.Count)" -ForegroundColor Yellow

# Kill all dotnet and Pcf.* processes that might be locking files
$processesToKill = Get-Process | Where-Object {
    ($_.ProcessName -eq "dotnet" -or $_.ProcessName -like "Pcf.*") -and
    $_.Path -and
    (Test-Path $_.Path)
}

if ($processesToKill.Count -eq 0) {
    Write-Host "No dotnet or Pcf.* processes found" -ForegroundColor Yellow
    exit 0
}

Write-Host "Found processes to kill: $($processesToKill.Count)" -ForegroundColor Yellow

# Kill processes
$killedCount = 0
foreach ($proc in $processesToKill) {
    try {
        Write-Host "Killing process: $($proc.ProcessName) (PID: $($proc.Id))" -ForegroundColor Yellow
        Stop-Process -Id $proc.Id -Force -ErrorAction Stop
        Write-Host "Process $($proc.ProcessName) (PID: $($proc.Id)) killed" -ForegroundColor Green
        $killedCount++
    }
    catch {
        Write-Host "Failed to kill process $($proc.ProcessName) (PID: $($proc.Id)): $_" -ForegroundColor Red
    }
}

Write-Host "Killed processes: $killedCount of $($processesToKill.Count)" -ForegroundColor Green
Write-Host "Done!" -ForegroundColor Green
