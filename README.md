# TravelRequests

Sistema de gestión de solicitudes de viaje desarrollado con .NET 6+ y arquitectura limpia.

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
