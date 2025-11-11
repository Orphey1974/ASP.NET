# PowerShell wrapper for running Python script to convert MD to DOCX
# Used for F5 launch in VS Code

param(
    [string]$FilePath = $null
)

$ErrorActionPreference = "Stop"

# If file not provided, try to get from environment or arguments
if (-not $FilePath) {
    # In VS Code ${file} variable is passed as argument
    if ($args.Count -gt 0) {
        $FilePath = $args[0]
    }
}

if (-not $FilePath -or -not (Test-Path $FilePath)) {
    Write-Host "Error: File not specified or not found" -ForegroundColor Red
    Write-Host "Usage: .\convert_md_to_docx.ps1 <path_to_file.md>" -ForegroundColor Yellow
    exit 1
}

# Check that file has .md extension
if (-not $FilePath.EndsWith('.md', [System.StringComparison]::OrdinalIgnoreCase)) {
    Write-Host "Error: File must have .md extension" -ForegroundColor Red
    exit 1
}

Write-Host "Converting file: $FilePath" -ForegroundColor Cyan
Write-Host ""

# Get path to Python script
$scriptPath = Join-Path $PSScriptRoot "convert_md_to_docx.py"

if (-not (Test-Path $scriptPath)) {
    Write-Host "Error: Script convert_md_to_docx.py not found" -ForegroundColor Red
    exit 1
}

# Run Python script
try {
    python $scriptPath $FilePath
    $exitCode = $LASTEXITCODE

    if ($exitCode -eq 0) {
        Write-Host ""
        Write-Host "Conversion completed successfully!" -ForegroundColor Green
    } else {
        Write-Host ""
        Write-Host "Error during conversion (exit code: $exitCode)" -ForegroundColor Red
    }

    exit $exitCode
}
catch {
    Write-Host "Error running Python script: $_" -ForegroundColor Red
    exit 1
}

