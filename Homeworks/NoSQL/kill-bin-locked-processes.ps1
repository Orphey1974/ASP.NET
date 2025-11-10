# Script to kill processes locking files in bin folder
param(
    [string]$ProjectDirectory = $PSScriptRoot
)

# Clear-Host only if running interactively
if ($Host.Name -eq 'ConsoleHost') {
    Clear-Host
}

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

# Get current process and parent process IDs to exclude them
$currentProcessId = $PID
$parentProcessId = (Get-CimInstance Win32_Process -Filter "ProcessId = $currentProcessId" | Select-Object -ExpandProperty ParentProcessId)
$excludedProcessIds = @($currentProcessId, $parentProcessId)

# Get all dotnet processes that might be locking files
$allDotnetProcesses = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Where-Object {
    $_.Path -and (Test-Path $_.Path)
}

# Filter out processes that are part of the build
# Exclude processes that are:
# 1. Current process or its parent
# 2. Processes with "build" or "msbuild" in command line (part of build)
$processesToKill = @()
foreach ($proc in $allDotnetProcesses) {
    if ($proc.Id -in $excludedProcessIds) {
        continue
    }

    # Check command line to see if it's a build process
    try {
        $cmdLine = (Get-CimInstance Win32_Process -Filter "ProcessId = $($proc.Id)" | Select-Object -ExpandProperty CommandLine)
        if ($cmdLine -and ($cmdLine -like "*build*" -or $cmdLine -like "*msbuild*" -or $cmdLine -like "*restore*")) {
            Write-Host "Skipping build process: PID $($proc.Id) - $($cmdLine.Substring(0, [Math]::Min(80, $cmdLine.Length)))" -ForegroundColor Gray
            continue
        }
    }
    catch {
        # If we can't get command line, be safe and skip it
        Write-Host "Skipping process (cannot check): PID $($proc.Id)" -ForegroundColor Gray
        continue
    }

    $processesToKill += $proc
}

# Also check for Pcf.* processes (if any)
$pcfProcesses = Get-Process | Where-Object {
    $_.ProcessName -like "Pcf.*" -and
    $_.Path -and
    (Test-Path $_.Path)
}

if ($pcfProcesses) {
    $processesToKill = $processesToKill + $pcfProcesses
}

if ($processesToKill.Count -eq 0) {
    Write-Host "No processes to kill (excluding build process)" -ForegroundColor Green
    Write-Host "  Current process PID: $currentProcessId" -ForegroundColor Gray
    Write-Host "  Parent process PID: $parentProcessId" -ForegroundColor Gray
    exit 0
}

Write-Host "Found processes to kill: $($processesToKill.Count)" -ForegroundColor Yellow
Write-Host "  Excluded build process PID: $parentProcessId" -ForegroundColor Gray

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
