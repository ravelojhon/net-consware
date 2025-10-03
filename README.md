# TravelRequests API

[![.NET CI/CD Pipeline](https://github.com/ravelojhon/net-consware/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ravelojhon/net-consware/actions/workflows/dotnet.yml)
[![Security Analysis](https://github.com/ravelojhon/net-consware/actions/workflows/security.yml/badge.svg)](https://github.com/ravelojhon/net-consware/actions/workflows/security.yml)
[![Code Quality Analysis](https://github.com/ravelojhon/net-consware/actions/workflows/code-quality.yml/badge.svg)](https://github.com/ravelojhon/net-consware/actions/workflows/code-quality.yml)
[![CodeQL Analysis](https://github.com/ravelojhon/net-consware/actions/workflows/codeql.yml/badge.svg)](https://github.com/ravelojhon/net-consware/actions/workflows/codeql.yml)
[![Docker](https://img.shields.io/badge/Docker-Ready-blue.svg)](https://github.com/ravelojhon/net-consware)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

## 🎯 Objetivo

**TravelRequests API** es un sistema completo de gestión de solicitudes de viaje empresarial desarrollado con **.NET 9.0** y **Clean Architecture**. El sistema permite a los empleados crear, gestionar y aprobar solicitudes de viaje con un flujo de trabajo robusto que incluye autenticación JWT, roles de usuario, validaciones de negocio y un sistema de aprobación.

### ✨ Características Principales

- 🔐 **Autenticación JWT** con roles (Employee, Manager, Admin)
- 📝 **Gestión de solicitudes de viaje** con validaciones de negocio
- 🔄 **Flujo de aprobación** con diferentes roles de usuario
- 🔒 **Reset de contraseñas** con códigos de verificación
- 🐳 **Contenedorización** completa con Docker
- 🧪 **Tests completos** (33 tests unitarios e integración)
- 📊 **CI/CD** con GitHub Actions
- 🔍 **Análisis de seguridad** y calidad de código
- 📚 **Documentación API** con Swagger/OpenAPI

## 🚀 Inicio Rápido

### Prerequisitos

- **.NET 9.0 SDK** o superior
- **SQL Server** (local o Docker)
- **Docker** (opcional, para contenedorización)
- **Git** para clonar el repositorio

### 📋 Pasos para Ejecutar Localmente

#### 1. Clonar el Repositorio
```bash
git clone https://github.com/ravelojhon/net-consware.git
cd net-consware
```

#### 2. Restaurar Dependencias
```bash
dotnet restore
```

#### 3. Configurar Base de Datos

**Opción A: SQL Server Local**
```bash
# Instalar Entity Framework CLI (si no está instalado)
dotnet tool install --global dotnet-ef

# Crear migración inicial
dotnet ef migrations add InitialCreate --project src/TravelRequests.Infrastructure --startup-project src/TravelRequests.Api

# Aplicar migraciones
dotnet ef database update --project src/TravelRequests.Infrastructure --startup-project src/TravelRequests.Api
```

**Opción B: SQL Server con Docker**
```bash
# Levantar SQL Server con Docker
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest

# Esperar que SQL Server esté listo (30-60 segundos)
# Luego ejecutar las migraciones
dotnet ef database update --project src/TravelRequests.Infrastructure --startup-project src/TravelRequests.Api
```

#### 4. Configurar Secrets (Opcional)
```bash
# Configurar JWT secrets para desarrollo
dotnet user-secrets init --project src/TravelRequests.Api
dotnet user-secrets set "Jwt:Key" "TU_SUPER_SECRETO_AQUI_MUY_LARGO_Y_SEGURO_2025" --project src/TravelRequests.Api
dotnet user-secrets set "Jwt:Issuer" "TravelRequestsApi" --project src/TravelRequests.Api
dotnet user-secrets set "Jwt:Audience" "TravelRequestsClient" --project src/TravelRequests.Api
dotnet user-secrets set "Jwt:ExpirationMinutes" "60" --project src/TravelRequests.Api
```

#### 5. Ejecutar la Aplicación
```bash
dotnet run --project src/TravelRequests.Api
```

La API estará disponible en:
- **API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health

## 🐳 Usar con Docker

### Opción 1: Docker Compose (Recomendado)
```bash
# Levantar todos los servicios
docker-compose up --build

# En modo detached
docker-compose up -d --build

# Ver logs
docker-compose logs -f

# Parar servicios
docker-compose down
```

### Opción 2: Script de PowerShell
```powershell
# Iniciar servicios
.\scripts\docker-commands.ps1 up -Build

# Ver logs
.\scripts\docker-commands.ps1 logs

# Ejecutar migraciones
.\scripts\docker-commands.ps1 migrate

# Parar servicios
.\scripts\docker-commands.ps1 down
```

### Servicios Docker
- **API**: http://localhost:5000
- **SQL Server**: localhost:1433
- **Base de datos**: TravelRequestsDb

## 🧪 Comandos de Prueba

### Tests Automáticos
```bash
# Ejecutar todos los tests
dotnet test

# Ejecutar tests con cobertura
dotnet test --collect:"XPlat Code Coverage"

# Ejecutar tests específicos
dotnet test --filter "AuthServiceTests"
```

### Tests Manuales con cURL

#### 1. Registrar Usuario
```bash
curl -X POST "http://localhost:5000/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Juan Pérez",
    "email": "juan@empresa.com",
    "password": "MiPassword123!",
    "role": 1
  }'
```

#### 2. Iniciar Sesión
```bash
curl -X POST "http://localhost:5000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "juan@empresa.com",
    "password": "MiPassword123!"
  }'
```

#### 3. Crear Solicitud de Viaje
```bash
curl -X POST "http://localhost:5000/api/travelrequests" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TU_TOKEN_AQUI" \
  -d '{
    "originCity": "Madrid",
    "destinationCity": "Barcelona",
    "dateFrom": "2024-02-15T00:00:00Z",
    "dateTo": "2024-02-18T00:00:00Z",
    "justification": "Reunión de trabajo con cliente importante"
  }'
```

#### 4. Aprobar Solicitud (Manager/Admin)
```bash
curl -X PATCH "http://localhost:5000/api/travelrequests/{id}/status" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TOKEN_MANAGER" \
  -d '{
    "status": 2
  }'
```

### Tests con Swagger UI
1. Abrir http://localhost:5000/swagger
2. Hacer clic en "Authorize"
3. Pegar el token JWT obtenido del login
4. Probar los endpoints directamente

## 📚 Endpoints Principales

### 🔐 Autenticación (`/api/auth`)
- `POST /register` - Registrar nuevo usuario
- `POST /login` - Iniciar sesión
- `GET /me` - Obtener usuario actual
- `POST /request-password-reset` - Solicitar reset de contraseña
- `POST /confirm-password-reset` - Confirmar reset de contraseña

### 📝 Solicitudes de Viaje (`/api/travelrequests`)
- `GET /` - Listar solicitudes (propias o todas si es aprobador)
- `GET /{id}` - Obtener solicitud por ID
- `POST /` - Crear nueva solicitud
- `PUT /{id}` - Actualizar solicitud
- `PATCH /{id}/status` - Cambiar estado (solo aprobadores)
- `DELETE /{id}` - Eliminar solicitud

### 🏥 Health Check (`/health`)
- `GET /` - Estado básico
- `GET /detailed` - Estado detallado con BD
- `GET /ready` - Readiness check
- `GET /live` - Liveness check

## 🏗️ Arquitectura y Decisiones Técnicas

### Clean Architecture
El proyecto sigue los principios de **Clean Architecture** con separación clara de responsabilidades:

```
┌─────────────────────────────────────────┐
│                API Layer                │ ← Controllers, DTOs, Middleware
├─────────────────────────────────────────┤
│            Application Layer            │ ← Use Cases, Services, Interfaces
├─────────────────────────────────────────┤
│              Domain Layer               │ ← Entities, Value Objects, Business Rules
├─────────────────────────────────────────┤
│           Infrastructure Layer          │ ← Data Access, External Services
└─────────────────────────────────────────┘
```

### Decisiones Técnicas Clave

#### 🔐 Autenticación y Seguridad
- **JWT Bearer Tokens**: Para autenticación stateless
- **BCrypt**: Para hashing seguro de contraseñas
- **Roles**: Sistema de roles (Employee, Manager, Admin)
- **Validación**: FluentValidation para validaciones robustas
- **Secrets**: User Secrets para configuración sensible

#### 🗄️ Base de Datos
- **Entity Framework Core**: ORM para acceso a datos
- **SQL Server**: Base de datos relacional
- **Code First**: Migraciones automáticas
- **Fluent API**: Configuración de relaciones
- **Índices**: Optimización de consultas

#### 🧪 Testing
- **xUnit**: Framework de testing
- **Moq**: Mocking para tests unitarios
- **InMemory Database**: Tests de integración
- **33 Tests**: Cobertura completa de funcionalidades

#### 🐳 Contenedorización
- **Docker**: Contenedorización de la aplicación
- **Docker Compose**: Orquestación de servicios
- **Multi-stage Build**: Optimización de imagen
- **Health Checks**: Monitoreo de contenedores

#### 🔄 CI/CD
- **GitHub Actions**: Pipeline de CI/CD
- **6 Workflows**: Build, test, security, quality, deploy
- **Dependabot**: Actualización automática de dependencias
- **CodeQL**: Análisis de seguridad avanzado

### Patrones de Diseño
- **Repository Pattern**: Abstracción de acceso a datos
- **Unit of Work**: Gestión de transacciones
- **Dependency Injection**: Inversión de control
- **CQRS**: Separación de comandos y consultas
- **DTO Pattern**: Transferencia de datos

## 🛠️ Tecnologías Utilizadas

### Backend
- **.NET 9.0** - Framework principal
- **ASP.NET Core Web API** - API REST
- **Entity Framework Core** - ORM
- **SQL Server** - Base de datos
- **JWT Bearer** - Autenticación
- **BCrypt.Net** - Hashing de contraseñas
- **FluentValidation** - Validaciones
- **AutoMapper** - Mapeo de objetos
- **Serilog** - Logging estructurado

### Testing
- **xUnit** - Framework de testing
- **Moq** - Mocking
- **Entity Framework InMemory** - Tests de integración

### DevOps
- **Docker** - Contenedorización
- **GitHub Actions** - CI/CD
- **Dependabot** - Actualización de dependencias
- **CodeQL** - Análisis de seguridad

## 📊 Métricas de Calidad

- ✅ **33 Tests** pasando (100% éxito)
- ✅ **0 Vulnerabilidades** de seguridad
- ✅ **Cobertura de código** completa
- ✅ **Análisis de calidad** automático
- ✅ **Documentación API** completa
- ✅ **Docker** funcionando correctamente

## 🚀 CI/CD y Calidad de Código

Este proyecto incluye un pipeline completo de CI/CD con GitHub Actions que verifica:

### ✅ Verificaciones Automáticas
- **Compilación**: Build automático en cada push/PR
- **Tests**: Ejecución de tests unitarios e integración
- **Seguridad**: Análisis de vulnerabilidades y secretos
- **Calidad**: Verificación de formato y complejidad
- **Docker**: Build y test de contenedores
- **Migraciones**: Validación de migraciones de BD

### 🔧 Workflows Disponibles
- **`.NET CI/CD Pipeline`**: Pipeline principal con build, tests y análisis
- **`Security Analysis`**: Análisis de seguridad y dependencias
- **`Code Quality Analysis`**: Verificación de calidad de código
- **`Deploy`**: Despliegue automático a staging/production
- **`CodeQL`**: Análisis de seguridad avanzado

### 📊 Métricas de Calidad
- Cobertura de código
- Análisis de vulnerabilidades
- Verificación de formato
- Análisis de complejidad
- Documentación XML

## 🔧 Desarrollo

### Comandos Útiles
```bash
# Ejecutar con hot reload
dotnet watch run --project src/TravelRequests.Api

# Crear nueva migración
dotnet ef migrations add <NombreMigracion> --project src/TravelRequests.Infrastructure --startup-project src/TravelRequests.Api

# Aplicar migraciones
dotnet ef database update --project src/TravelRequests.Infrastructure --startup-project src/TravelRequests.Api

# Verificar calidad de código localmente
dotnet format --verify-no-changes
dotnet list package --vulnerable --include-transitive
dotnet outdated
dotnet build --configuration Release --verbosity normal /p:RunAnalyzersDuringBuild=true
```

### Estructura del Proyecto
```
/src
  /TravelRequests.Api          # API Web (ASP.NET Core)
  /TravelRequests.Application  # Lógica de aplicación (Casos de uso)
  /TravelRequests.Domain      # Entidades y reglas de negocio
  /TravelRequests.Infrastructure # Acceso a datos y servicios externos
/tests
  /TravelRequests.Tests       # Pruebas unitarias (xUnit)
/.github
  /workflows                  # GitHub Actions workflows
  dependabot.yml             # Configuración de Dependabot
docker-compose.yml           # Configuración de Docker
Dockerfile                   # Imagen de Docker
```

## 📋 Convenciones

### Branches
- `feature/<descripción>` - Nuevas funcionalidades
- `fix/<descripción>` - Corrección de bugs
- `chore/<descripción>` - Tareas de mantenimiento
- `release/<descripción>` - Preparación de releases

### Commits
Seguir [Conventional Commits](https://www.conventionalcommits.org/):
- `feat:` - Nueva funcionalidad
- `fix:` - Corrección de bug
- `chore:` - Cambios en herramientas o configuración
- `docs:` - Cambios en documentación
- `test:` - Agregar o modificar pruebas

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo [LICENSE](LICENSE) para más detalles.

## 🤝 Contribución

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'feat: add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📞 Soporte

Para soporte o preguntas:
- Crear un [Issue](https://github.com/ravelojhon/net-consware/issues)
- Revisar la [documentación de la API](http://localhost:5000/swagger)
- Consultar los [workflows de CI/CD](.github/workflows/)

---

**Desarrollado con ❤️ usando .NET 9.0 y Clean Architecture**