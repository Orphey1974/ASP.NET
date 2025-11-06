# Markdown Error Fix Script - Simplified Version
# Author: AI Assistant
# Description: Automatically fixes common markdown formatting issues

param(
    [string]$Path = ".",
    [switch]$Recursive = $false,
    [switch]$DryRun = $false,
    [switch]$Verbose = $false
)

# Logging function
function Write-Log {
    param([string]$Message, [string]$Level = "INFO")
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $color = switch ($Level) {
        "ERROR" { "Red" }
        "WARNING" { "Yellow" }
        "SUCCESS" { "Green" }
        default { "White" }
    }
    Write-Host "[$timestamp] [$Level] $Message" -ForegroundColor $color
}

# Get markdown files function
function Get-MarkdownFiles {
    param([string]$SearchPath, [bool]$IsRecursive)
    
    if ($IsRecursive) {
        return Get-ChildItem -Path $SearchPath -Filter "*.md" -Recurse -File
    } else {
        return Get-ChildItem -Path $SearchPath -Filter "*.md" -File
    }
}

# Fix markdown content function - simplified
function Fix-MarkdownContent {
    param([string]$Content)
    
    $fixedContent = $Content
    
    # 1. Remove trailing spaces from lines
    $fixedContent = $fixedContent -replace '\s+\r?\n', "`n"
    
    # 2. Fix multiple empty lines - replace with max 2 empty lines
    $fixedContent = $fixedContent -replace '\n{3,}', "`n`n"
    
    # 3. Add empty line before headers (except first line)
    $lines = $fixedContent -split "`n"
    $result = @()
    
    for ($i = 0; $i -lt $lines.Length; $i++) {
        $line = $lines[$i]
        
        # Add empty line before headers (except first line)
        if ($line -match '^#+\s' -and $i -gt 0) {
            $prevLine = if ($i -gt 0) { $lines[$i - 1] } else { "" }
            if ($prevLine -ne "" -and $prevLine -ne "`n") {
                $result += ""
            }
        }
        
        # Add empty line before lists
        if ($line -match '^\s*[-*+]\s' -and $i -gt 0) {
            $prevLine = if ($i -gt 0) { $lines[$i - 1] } else { "" }
            if ($prevLine -ne "" -and $prevLine -ne "`n") {
                $result += ""
            }
        }
        
        # Add empty line before numbered lists
        if ($line -match '^\s*\d+\.\s' -and $i -gt 0) {
            $prevLine = if ($i -gt 0) { $lines[$i - 1] } else { "" }
            if ($prevLine -ne "" -and $prevLine -ne "`n") {
                $result += ""
            }
        }
        
        # Add empty line before code blocks
        if ($line -match '^```' -and $i -gt 0) {
            $prevLine = if ($i -gt 0) { $lines[$i - 1] } else { "" }
            if ($prevLine -ne "" -and $prevLine -ne "`n") {
                $result += ""
            }
        }
        
        # Add empty line before tables
        if ($line -match '^\|.*\|$' -and $i -gt 0) {
            $prevLine = if ($i -gt 0) { $lines[$i - 1] } else { "" }
            if ($prevLine -ne "" -and $prevLine -ne "`n") {
                $result += ""
            }
        }
        
        # Add empty line before horizontal rules
        if ($line -match '^(---+|\*\*\*+|___+)$' -and $i -gt 0) {
            $prevLine = if ($i -gt 0) { $lines[$i - 1] } else { "" }
            if ($prevLine -ne "" -and $prevLine -ne "`n") {
                $result += ""
            }
        }
        
        # Add empty line before blockquotes
        if ($line -match '^>\s' -and $i -gt 0) {
            $prevLine = if ($i -gt 0) { $lines[$i - 1] } else { "" }
            if ($prevLine -ne "" -and $prevLine -ne "`n") {
                $result += ""
            }
        }
        
        $result += $line
    }
    
    $fixedContent = $result -join "`n"
    
    # 4. Trim start and end of file
    $fixedContent = $fixedContent.Trim()
    
    # 5. Add final newline if missing
    if (-not $fixedContent.EndsWith("`n")) {
        $fixedContent += "`n"
    }
    
    return $fixedContent
}

# Process markdown file function
function Process-MarkdownFile {
    param([System.IO.FileInfo]$File)
    
    try {
        Write-Log "Processing file: $($File.Name)" "INFO"
        
        # Read file content with correct encoding
        $content = Get-Content -Path $File.FullName -Raw -Encoding UTF8
        
        if ($null -eq $content) {
            Write-Log "File is empty or could not be read: $($File.FullName)" "WARNING"
            return $false
        }
        
        # Fix content
        $fixedContent = Fix-MarkdownContent -Content $content
        
        # Check for changes
        if ($content -ne $fixedContent) {
            if ($DryRun) {
                Write-Log "DRY RUN: File would be modified: $($File.Name)" "WARNING"
                
                if ($Verbose) {
                    Write-Log "Sample changes (first 5 differences):" "INFO"
                    
                    # Show sample differences
                    $originalLines = $content -split "`n"
                    $fixedLines = $fixedContent -split "`n"
                    
                    $diffCount = 0
                    for ($i = 0; $i -lt [Math]::Max($originalLines.Length, $fixedLines.Length) -and $diffCount -lt 5; $i++) {
                        $originalLine = if ($i -lt $originalLines.Length) { $originalLines[$i] } else { "" }
                        $fixedLine = if ($i -lt $fixedLines.Length) { $fixedLines[$i] } else { "" }
                        
                        if ($originalLine -ne $fixedLine) {
                            Write-Host "  Line $($i + 1):" -ForegroundColor Yellow
                            if ($originalLine) { Write-Host "    -: $originalLine" -ForegroundColor Red }
                            if ($fixedLine) { Write-Host "    +: $fixedLine" -ForegroundColor Green }
                            $diffCount++
                        }
                    }
                }
            } else {
                # Save fixed file
                Set-Content -Path $File.FullName -Value $fixedContent -Encoding UTF8 -NoNewline
                Write-Log "File fixed: $($File.Name)" "SUCCESS"
            }
            return $true
        } else {
            Write-Log "File does not need changes: $($File.Name)" "INFO"
            return $false
        }
        
    } catch {
        Write-Log "Error processing file $($File.Name): $($_.Exception.Message)" "ERROR"
        return $false
    }
}

# Main function
function Main {
    Write-Log "Starting markdown error fix script" "INFO"
    Write-Log "Path: $Path" "INFO"
    Write-Log "Recursive search: $Recursive" "INFO"
    Write-Log "Dry run mode: $DryRun" "INFO"
    
    # Check if path exists
    if (-not (Test-Path -Path $Path)) {
        Write-Log "Path does not exist: $Path" "ERROR"
        return
    }
    
    # Get all markdown files
    $markdownFiles = Get-MarkdownFiles -SearchPath $Path -IsRecursive $Recursive
    
    if ($markdownFiles.Count -eq 0) {
        Write-Log "No markdown files found in specified path" "WARNING"
        return
    }
    
    Write-Log "Found markdown files: $($markdownFiles.Count)" "INFO"
    
    # Process each file
    $processedCount = 0
    $modifiedCount = 0
    
    foreach ($file in $markdownFiles) {
        $wasModified = Process-MarkdownFile -File $file
        if ($wasModified) {
            $modifiedCount++
        }
        $processedCount++
    }
    
    Write-Log "Processing completed" "SUCCESS"
    Write-Log "Files processed: $processedCount" "INFO"
    
    if (-not $DryRun) {
        Write-Log "Files modified: $modifiedCount" "INFO"
    } else {
        Write-Log "Files that would be modified: $modifiedCount" "INFO"
    }
}

# Run main function
Main
