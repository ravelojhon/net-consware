# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-01-10

### Added
- 🎯 **Sistema completo de gestión de solicitudes de viaje**
- 🔐 **Autenticación JWT** con roles (Employee, Manager, Admin)
- 📝 **CRUD completo** para solicitudes de viaje
- 🔄 **Flujo de aprobación** con diferentes roles
- 🔒 **Sistema de reset de contraseñas** con códigos de verificación
- 🐳 **Contenedorización completa** con Docker y Docker Compose
- 🧪 **33 tests** (unitarios e integración) con 100% de éxito
- 📊 **Pipeline CI/CD** completo con GitHub Actions
- 🔍 **Análisis de seguridad** y calidad de código automático
- 📚 **Documentación API** completa con Swagger/OpenAPI
- 🏗️ **Clean Architecture** con separación clara de responsabilidades

### Technical Features
- **Backend**: .NET 9.0, ASP.NET Core Web API, Entity Framework Core
- **Database**: SQL Server con migraciones automáticas
- **Security**: JWT Bearer, BCrypt, FluentValidation
- **Testing**: xUnit, Moq, Entity Framework InMemory
- **DevOps**: Docker, GitHub Actions, Dependabot, CodeQL
- **Logging**: Serilog con archivos y consola
- **Documentation**: Swagger/OpenAPI con autenticación JWT

### API Endpoints
- **Authentication**: `/api/auth` (register, login, password reset)
- **Travel Requests**: `/api/travelrequests` (CRUD + approval workflow)
- **Health Check**: `/health` (basic, detailed, ready, live)

### Docker Support
- **Multi-stage Dockerfile** optimizado
- **Docker Compose** con SQL Server y API
- **Health checks** para monitoreo
- **Scripts de PowerShell** para gestión fácil

### CI/CD Pipeline
- **6 workflows** de GitHub Actions
- **Build, test, security, quality, deploy**
- **Análisis automático** de vulnerabilidades
- **Actualización automática** de dependencias
- **Deployment** a staging y production

### Quality Assurance
- **33 tests** pasando (100% éxito)
- **0 vulnerabilidades** de seguridad
- **Cobertura de código** completa
- **Análisis de calidad** automático
- **Documentación** verificada automáticamente

## [0.1.0] - 2025-01-10

### Added
- 🏗️ **Estructura inicial** del proyecto con Clean Architecture
- 📦 **Configuración base** de .NET 9.0 y dependencias
- 🗄️ **Modelo de datos** inicial (User, TravelRequest, PasswordResetCode)
- 🔧 **Configuración** de Entity Framework Core
- 📝 **README** básico del proyecto

---

## Versioning

This project uses [Semantic Versioning](https://semver.org/). For the versions available, see the [tags on this repository](https://github.com/ravelojhon/net-consware/tags).

## Release Notes

### v1.0.0 - Initial Release
This is the first stable release of TravelRequests API, featuring a complete travel request management system with authentication, approval workflows, Docker support, comprehensive testing, and full CI/CD pipeline.

**Key Highlights:**
- ✅ Production-ready API with 33 passing tests
- ✅ Complete Docker containerization
- ✅ Full CI/CD pipeline with security analysis
- ✅ Comprehensive documentation and Swagger UI
- ✅ Clean Architecture implementation
- ✅ Zero security vulnerabilities
- ✅ Automated dependency updates

**Ready for:**
- 🚀 Production deployment
- 👥 Team collaboration
- 🔄 Continuous integration
- 📈 Scalability and maintenance
