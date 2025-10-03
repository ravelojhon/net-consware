# TravelRequests

[![.NET CI/CD Pipeline](https://github.com/ravelojhon/net-consware/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ravelojhon/net-consware/actions/workflows/dotnet.yml)
[![Security Analysis](https://github.com/ravelojhon/net-consware/actions/workflows/security.yml/badge.svg)](https://github.com/ravelojhon/net-consware/actions/workflows/security.yml)
[![Code Quality Analysis](https://github.com/ravelojhon/net-consware/actions/workflows/code-quality.yml/badge.svg)](https://github.com/ravelojhon/net-consware/actions/workflows/code-quality.yml)
[![CodeQL Analysis](https://github.com/ravelojhon/net-consware/actions/workflows/codeql.yml/badge.svg)](https://github.com/ravelojhon/net-consware/actions/workflows/codeql.yml)
[![Docker](https://img.shields.io/badge/Docker-Ready-blue.svg)](https://github.com/ravelojhon/net-consware)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

Sistema de gestión de solicitudes de viaje desarrollado con .NET 9.0 y arquitectura limpia.

## Estructura del Proyecto

```
/src
  /TravelRequests.Api          # API Web (ASP.NET Core)
  /TravelRequests.Application  # Lógica de aplicación (Casos de uso)
  /TravelRequests.Domain      # Entidades y reglas de negocio
  /TravelRequests.Infrastructure # Acceso a datos y servicios externos
/tests
  /TravelRequests.Tests       # Pruebas unitarias (xUnit)
```

## Prerequisitos

- .NET SDK 6+ instalado
- SQL Server local `(localdb)\mssqllocaldb` o contenedor Docker
- Entity Framework Core CLI:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

## Configuración

1. **Clonar el repositorio**
   ```bash
   git clone <url-del-repositorio>
   cd TravelRequests
   ```

2. **Restaurar dependencias**
   ```bash
   dotnet restore
   ```

3. **Configurar base de datos**
   ```bash
   # Crear migración inicial
   dotnet ef migrations add InitialCreate --project src/TravelRequests.Infrastructure --startup-project src/TravelRequests.Api
   
   # Aplicar migraciones
   dotnet ef database update --project src/TravelRequests.Infrastructure --startup-project src/TravelRequests.Api
   ```

4. **Ejecutar la aplicación**
   ```bash
   dotnet run --project src/TravelRequests.Api
   ```

## Convenciones

### Branches
- `feature/<descripción>` - Nuevas funcionalidades
- `fix/<descripción>` - Corrección de bugs
- `chore/<descripción>` - Tareas de mantenimiento

### Commits
Seguir [Conventional Commits](https://www.conventionalcommits.org/):
- `feat:` - Nueva funcionalidad
- `fix:` - Corrección de bug
- `chore:` - Cambios en herramientas o configuración
- `docs:` - Cambios en documentación
- `test:` - Agregar o modificar pruebas

### Ejemplos de commits:
```bash
feat: agregar endpoint para crear solicitud de viaje
fix: corregir validación de fechas en solicitudes
chore: actualizar dependencias de Entity Framework
docs: actualizar README con instrucciones de Docker
test: agregar pruebas unitarias para servicio de validación
```

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

## Desarrollo

### Ejecutar pruebas
```bash
dotnet test
```

### Ejecutar con hot reload
```bash
dotnet watch run --project src/TravelRequests.Api
```

### Crear nueva migración
```bash
dotnet ef migrations add <NombreMigracion> --project src/TravelRequests.Infrastructure --startup-project src/TravelRequests.Api
```

### Verificar calidad de código localmente
```bash
# Verificar formato
dotnet format --verify-no-changes

# Verificar vulnerabilidades
dotnet list package --vulnerable --include-transitive

# Verificar dependencias obsoletas
dotnet outdated

# Build con análisis
dotnet build --configuration Release --verbosity normal /p:RunAnalyzersDuringBuild=true
```

## Arquitectura

El proyecto sigue los principios de **Clean Architecture**:

- **Domain**: Entidades, value objects y reglas de negocio
- **Application**: Casos de uso, interfaces y DTOs
- **Infrastructure**: Implementación de repositorios, base de datos y servicios externos
- **API**: Controladores, configuración y punto de entrada

## Tecnologías

- .NET 6+
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- xUnit (pruebas)
- AutoMapper (mapeo de objetos)
- FluentValidation (validaciones)
