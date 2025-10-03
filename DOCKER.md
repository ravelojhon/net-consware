# Docker Setup for Travel Requests API

This document explains how to run the Travel Requests API using Docker and Docker Compose.

## Prerequisites

- Docker Desktop (Windows/Mac) or Docker Engine (Linux)
- Docker Compose v2.0+

## Quick Start

### 1. Start the Application

```bash
# Start all services (API + SQL Server)
docker-compose up --build

# Or use the PowerShell script
.\scripts\docker-commands.ps1 up -Build
```

### 2. Access the Application

- **API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **SQL Server**: localhost:1433

## Services

### SQL Server
- **Image**: mcr.microsoft.com/mssql/server:2022-latest
- **Port**: 1433
- **Database**: TravelRequestsDb
- **Username**: sa
- **Password**: YourStrong@Passw0rd

### API
- **Port**: 5000 (mapped to 80 inside container)
- **Environment**: Production
- **Health Check**: http://localhost:5000/health

## Available Commands

### Using Docker Compose

```bash
# Start services
docker-compose up

# Start in detached mode
docker-compose up -d

# Build and start
docker-compose up --build

# Stop services
docker-compose down

# View logs
docker-compose logs -f

# Run database migrations
docker-compose --profile migration run --rm migration

# Check service status
docker-compose ps
```

### Using PowerShell Script

```powershell
# Start services
.\scripts\docker-commands.ps1 up

# Start in detached mode
.\scripts\docker-commands.ps1 up -Detached

# Build and start
.\scripts\docker-commands.ps1 up -Build

# Stop services
.\scripts\docker-commands.ps1 down

# View logs
.\scripts\docker-commands.ps1 logs

# Run migrations
.\scripts\docker-commands.ps1 migrate

# Clean up resources
.\scripts\docker-commands.ps1 clean

# Check status
.\scripts\docker-commands.ps1 status

# Restart services
.\scripts\docker-commands.ps1 restart
```

## Database Management

### Running Migrations

The application includes automatic database migration on startup. However, you can also run migrations manually:

```bash
# Using Docker Compose
docker-compose --profile migration run --rm migration

# Using PowerShell script
.\scripts\docker-commands.ps1 migrate
```

### Connecting to SQL Server

You can connect to the SQL Server instance using any SQL client:

- **Server**: localhost,1433
- **Database**: TravelRequestsDb
- **Username**: sa
- **Password**: YourStrong@Passw0rd
- **Encrypt**: No (for development)

## Environment Variables

The following environment variables are configured in docker-compose.yml:

### API Service
- `ASPNETCORE_ENVIRONMENT=Production`
- `ASPNETCORE_URLS=http://+:80`
- `ConnectionStrings__DefaultConnection=Server=sqlserver,1433;Database=TravelRequestsDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;MultipleActiveResultSets=true`
- `Jwt__Key=TU_SUPER_SECRETO_AQUI_MUY_LARGO_Y_SEGURO_2025_DOCKER`
- `Jwt__Issuer=TravelRequestsApi`
- `Jwt__Audience=TravelRequestsClient`
- `Jwt__ExpirationMinutes=60`

## Volumes

- `sqlserver_data`: Persistent storage for SQL Server data
- `api_logs`: Application logs storage

## Networks

- `travelrequests-network`: Internal network for service communication

## Health Checks

Both services include health checks:

- **SQL Server**: Checks database connectivity
- **API**: Checks HTTP endpoint availability

## Troubleshooting

### Common Issues

1. **Port already in use**
   ```bash
   # Check what's using the port
   netstat -ano | findstr :5000
   netstat -ano | findstr :1433
   
   # Stop conflicting services
   docker-compose down
   ```

2. **Database connection issues**
   ```bash
   # Check SQL Server logs
   docker-compose logs sqlserver
   
   # Restart SQL Server
   docker-compose restart sqlserver
   ```

3. **API not starting**
   ```bash
   # Check API logs
   docker-compose logs api
   
   # Rebuild API image
   docker-compose build --no-cache api
   ```

### Clean Up

```bash
# Stop and remove all containers, networks, and volumes
docker-compose down -v --remove-orphans

# Remove unused images
docker image prune -f

# Remove unused volumes
docker volume prune -f
```

## Development

### Making Changes

1. Make your code changes
2. Rebuild the API container:
   ```bash
   docker-compose build api
   docker-compose up api
   ```

### Debugging

To debug the application:

1. Attach a debugger to the running container
2. Check logs: `docker-compose logs -f api`
3. Access container shell: `docker-compose exec api /bin/bash`

## Production Considerations

For production deployment:

1. Change default passwords
2. Use environment-specific configuration
3. Enable SSL/TLS
4. Set up proper logging
5. Configure monitoring and alerting
6. Use secrets management
7. Set up backup strategies

## Security Notes

- Default passwords are used for development only
- Change all passwords in production
- Use secrets management for sensitive data
- Enable SSL/TLS in production
- Regularly update base images
