# Script to start databases via Docker Compose
# Starts only databases and infrastructure services needed for microservices

Write-Host "Starting databases for microservices..." -ForegroundColor Cyan
Write-Host ""

# Check for docker-compose
if (-not (Get-Command docker-compose -ErrorAction SilentlyContinue) -and -not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Host "Error: Docker is not installed or not available in PATH" -ForegroundColor Red
    exit 1
}

# Use docker compose (new version) or docker-compose (old version)
$dockerComposeCmd = if (Get-Command docker -ErrorAction SilentlyContinue) {
    docker compose
} else {
    docker-compose
}

Write-Host "Starting the following services:" -ForegroundColor Yellow
Write-Host "  - MongoDB for Administration" -ForegroundColor White
Write-Host "  - PostgreSQL for ReceivingFromPartner" -ForegroundColor White
Write-Host "  - PostgreSQL for GivingToCustomer" -ForegroundColor White
Write-Host "  - Redis for Preferences" -ForegroundColor White
Write-Host "  - RabbitMQ for message exchange" -ForegroundColor White
Write-Host ""

# Start databases and infrastructure services
& $dockerComposeCmd up -d `
    promocode-factory-administration-mongodb `
    promocode-factory-receiving-from-partner-db `
    promocode-factory-giving-to-customer-db `
    promocode-factory-redis `
    promocode-factory-rabbitmq

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "Databases started successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Available services:" -ForegroundColor Cyan
    Write-Host "  - MongoDB: localhost:27017" -ForegroundColor White
    Write-Host "  - PostgreSQL (ReceivingFromPartner): localhost:5434" -ForegroundColor White
    Write-Host "  - PostgreSQL (GivingToCustomer): localhost:5435" -ForegroundColor White
    Write-Host "  - Redis: localhost:6379" -ForegroundColor White
    Write-Host "  - RabbitMQ Management UI: http://localhost:15672 (admin/password)" -ForegroundColor White
    Write-Host "  - RabbitMQ AMQP: localhost:5672" -ForegroundColor White
    Write-Host ""
    Write-Host "To stop, run: docker-compose down" -ForegroundColor Yellow
} else {
    Write-Host ""
    Write-Host "Error starting databases!" -ForegroundColor Red
    exit 1
}
