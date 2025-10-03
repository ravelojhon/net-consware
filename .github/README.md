# GitHub Actions Workflows

Este directorio contiene los workflows de GitHub Actions para el proyecto Travel Requests API.

## 📋 Workflows Disponibles

### 🔄 CI/CD Principal
- **Archivo**: `dotnet.yml`
- **Trigger**: Push a cualquier rama, Pull Requests a main/develop
- **Descripción**: Pipeline principal que ejecuta build, tests, análisis de seguridad y calidad de código

### 🔒 Análisis de Seguridad
- **Archivo**: `security.yml`
- **Trigger**: Push a main/develop, Pull Requests, Programado (semanal)
- **Descripción**: Análisis de vulnerabilidades, dependencias obsoletas y secretos hardcodeados

### 📊 Análisis de Calidad de Código
- **Archivo**: `code-quality.yml`
- **Trigger**: Push a cualquier rama, Pull Requests a main/develop
- **Descripción**: Verificación de formato, análisis de complejidad y documentación

### 🚀 Deployment
- **Archivo**: `deploy.yml`
- **Trigger**: Push a main, Manual dispatch
- **Descripción**: Despliegue automático a staging y production

### 🔍 Análisis de Código con CodeQL
- **Archivo**: `codeql.yml`
- **Trigger**: Push a main/develop, Pull Requests, Programado (semanal)
- **Descripción**: Análisis de seguridad avanzado con GitHub CodeQL

### 📈 Actualización de Estado
- **Archivo**: `status.yml`
- **Trigger**: Completación de otros workflows
- **Descripción**: Actualización de badges de estado

## 🛠️ Configuración Adicional

### 🤖 Dependabot
- **Archivo**: `dependabot.yml`
- **Descripción**: Actualización automática de dependencias NuGet y GitHub Actions

## 📊 Métricas de Calidad

Los workflows verifican:

### ✅ Compilación y Tests
- [x] Compilación sin errores
- [x] Tests unitarios
- [x] Tests de integración
- [x] Cobertura de código

### 🔒 Seguridad
- [x] Vulnerabilidades en dependencias
- [x] Secretos hardcodeados
- [x] Análisis de código con CodeQL
- [x] Dependencias obsoletas

### 📊 Calidad de Código
- [x] Formato de código
- [x] Análisis de complejidad
- [x] Documentación XML
- [x] Uso de async/await
- [x] Gestión de recursos

### 🐳 Docker
- [x] Build de imagen Docker
- [x] Tests de contenedor
- [x] Health checks

### 🗄️ Base de Datos
- [x] Migraciones automáticas
- [x] Tests de conectividad
- [x] Validación de esquema

## 🚀 Uso

### Ejecutar Workflows Manualmente
```bash
# Ejecutar pipeline completo
gh workflow run "dotnet.yml"

# Ejecutar análisis de seguridad
gh workflow run "security.yml"

# Ejecutar análisis de calidad
gh workflow run "code-quality.yml"

# Ejecutar deployment
gh workflow run "deploy.yml"
```

### Ver Estado de Workflows
```bash
# Ver estado de todos los workflows
gh run list

# Ver logs de un workflow específico
gh run view <run-id>
```

## 📋 Requisitos

### Secrets Necesarios
Para el funcionamiento completo de los workflows, se requieren los siguientes secrets:

- `DOCKER_USERNAME`: Usuario de Docker Hub
- `DOCKER_PASSWORD`: Contraseña de Docker Hub

### Variables de Entorno
- `DOTNET_VERSION`: Versión de .NET (9.0.x)
- `DOTNET_SKIP_FIRST_TIME_EXPERIENCE`: true
- `DOTNET_CLI_TELEMETRY_OPTOUT`: true

## 🔧 Configuración Local

Para ejecutar los mismos checks localmente:

```bash
# Instalar herramientas
dotnet tool install --global dotnet-format
dotnet tool install --global security-scan
dotnet tool install --global dotnet-outdated

# Verificar formato
dotnet format --verify-no-changes

# Verificar vulnerabilidades
dotnet list package --vulnerable --include-transitive

# Verificar dependencias obsoletas
dotnet outdated

# Ejecutar tests
dotnet test

# Build con análisis
dotnet build --configuration Release --verbosity normal /p:RunAnalyzersDuringBuild=true
```

## 📈 Mejoras Futuras

- [ ] Integración con SonarQube
- [ ] Análisis de performance automatizado
- [ ] Tests de carga automatizados
- [ ] Notificaciones a Slack/Teams
- [ ] Métricas de cobertura de código
- [ ] Análisis de dependencias con Snyk
- [ ] Escaneo de contenedores con Trivy
