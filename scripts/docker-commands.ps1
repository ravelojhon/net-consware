# Docker Commands for Travel Requests API
# This script provides convenient commands for managing the Docker environment

param(
    [Parameter(Position=0)]
    [ValidateSet("up", "down", "build", "logs", "migrate", "clean", "status", "restart")]
    [string]$Command = "up",
    
    [switch]$Detached,
    [switch]$Build
)

$ProjectName = "travelrequests"

function Write-Info {
    param([string]$Message)
    Write-Host "ℹ️  $Message" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "✅ $Message" -ForegroundColor Green
}

function Write-Error {
    param([string]$Message)
    Write-Host "❌ $Message" -ForegroundColor Red
}

function Write-Warning {
    param([string]$Message)
    Write-Host "⚠️  $Message" -ForegroundColor Yellow
}

switch ($Command) {
    "up" {
        Write-Info "Starting Travel Requests API with Docker Compose..."
        $args = @("up")
        if ($Detached) { $args += "-d" }
        if ($Build) { $args += "--build" }
        
        docker-compose -p $ProjectName @args
        if ($LASTEXITCODE -eq 0) {
            Write-Success "Services started successfully!"
            Write-Info "API available at: http://localhost:5000"
            Write-Info "Swagger UI available at: http://localhost:5000/swagger"
            Write-Info "SQL Server available at: localhost:1433"
        } else {
            Write-Error "Failed to start services"
        }
    }
    
    "down" {
        Write-Info "Stopping Travel Requests API services..."
        docker-compose -p $ProjectName down
        Write-Success "Services stopped successfully!"
    }
    
    "build" {
        Write-Info "Building Travel Requests API Docker images..."
        docker-compose -p $ProjectName build --no-cache
        if ($LASTEXITCODE -eq 0) {
            Write-Success "Images built successfully!"
        } else {
            Write-Error "Failed to build images"
        }
    }
    
    "logs" {
        Write-Info "Showing logs for Travel Requests API services..."
        docker-compose -p $ProjectName logs -f
    }
    
    "migrate" {
        Write-Info "Running database migrations..."
        docker-compose -p $ProjectName --profile migration run --rm migration
        if ($LASTEXITCODE -eq 0) {
            Write-Success "Database migrations completed successfully!"
        } else {
            Write-Error "Failed to run database migrations"
        }
    }
    
    "clean" {
        Write-Warning "Cleaning up Docker resources..."
        Write-Info "Stopping and removing containers..."
        docker-compose -p $ProjectName down -v --remove-orphans
        
        Write-Info "Removing unused images..."
        docker image prune -f
        
        Write-Info "Removing unused volumes..."
        docker volume prune -f
        
        Write-Success "Cleanup completed!"
    }
    
    "status" {
        Write-Info "Checking status of Travel Requests API services..."
        docker-compose -p $ProjectName ps
    }
    
    "restart" {
        Write-Info "Restarting Travel Requests API services..."
        docker-compose -p $ProjectName restart
        Write-Success "Services restarted successfully!"
    }
    
    default {
        Write-Error "Unknown command: $Command"
        Write-Info "Available commands: up, down, build, logs, migrate, clean, status, restart"
        Write-Info "Usage: .\scripts\docker-commands.ps1 [command] [-Detached] [-Build]"
    }
}

Write-Info "Docker command completed."
