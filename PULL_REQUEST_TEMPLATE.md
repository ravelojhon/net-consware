# Pull Request

## 📋 Descripción
<!-- Proporciona una descripción clara y concisa de los cambios realizados -->

## 🔗 Tipo de cambio
<!-- Marca con [x] el tipo de cambio que aplica -->

- [ ] 🐛 Bug fix (cambio que corrige un problema)
- [ ] ✨ Nueva funcionalidad (cambio que agrega funcionalidad)
- [ ] 💥 Breaking change (cambio que puede causar que la funcionalidad existente no funcione como se esperaba)
- [ ] 📚 Documentación (cambio solo en documentación)
- [ ] 🎨 Mejora de código (refactoring, optimización, etc.)
- [ ] ⚡ Performance (cambio que mejora el rendimiento)
- [ ] 🔒 Seguridad (cambio relacionado con seguridad)
- [ ] 🧪 Tests (agregar o modificar tests)
- [ ] 🔧 CI/CD (cambios en configuración de CI/CD)
- [ ] 📦 Dependencias (actualización de paquetes NuGet)

## 🧪 Testing
<!-- Describe las pruebas que has realizado para verificar tus cambios -->

### ✅ Checklist de Testing
- [ ] **Compilación**: El código compila sin errores
- [ ] **Tests Unitarios**: Todos los tests unitarios pasan
- [ ] **Tests de Integración**: Los tests de integración pasan
- [ ] **Tests Manuales**: He probado manualmente la funcionalidad
- [ ] **Swagger**: La documentación de Swagger se genera correctamente
- [ ] **Base de Datos**: Las migraciones se ejecutan correctamente
- [ ] **Docker**: La aplicación funciona en Docker (si aplica)

### 🔍 Cómo probar
<!-- Describe paso a paso cómo probar los cambios -->

1. 
2. 
3. 

## 📊 Impacto
<!-- Describe el impacto de los cambios -->

### 🎯 Funcionalidades afectadas
- [ ] Autenticación (JWT)
- [ ] Gestión de usuarios
- [ ] Solicitudes de viaje
- [ ] Reset de contraseñas
- [ ] API endpoints
- [ ] Base de datos
- [ ] Docker/Contenedores
- [ ] Tests
- [ ] Documentación

### 🔄 Cambios en la API
<!-- Si hay cambios en la API, describe qué endpoints se modificaron -->

- [ ] No hay cambios en la API
- [ ] Se agregaron nuevos endpoints
- [ ] Se modificaron endpoints existentes
- [ ] Se eliminaron endpoints
- [ ] Se cambiaron los DTOs
- [ ] Se modificaron los códigos de respuesta HTTP

## 🔒 Seguridad
<!-- Checklist de seguridad -->

- [ ] **Contraseñas**: No hay contraseñas hardcodeadas
- [ ] **Secrets**: No hay secrets expuestos en el código
- [ ] **Validación**: Se validan todas las entradas del usuario
- [ ] **Autorización**: Se verifican los permisos correctamente
- [ ] **SQL Injection**: Se usan parámetros en las consultas SQL
- [ ] **XSS**: Se previenen ataques XSS
- [ ] **CSRF**: Se implementan protecciones CSRF (si aplica)
- [ ] **JWT**: Los tokens JWT se manejan de forma segura

## 📚 Documentación
<!-- Checklist de documentación -->

- [ ] **README**: Se actualizó el README si es necesario
- [ ] **Comentarios**: Se agregaron comentarios XML para Swagger
- [ ] **Código**: El código está bien documentado
- [ ] **API Docs**: La documentación de la API está actualizada
- [ ] **Docker**: Se actualizó la documentación de Docker si aplica

## 🗄️ Base de Datos
<!-- Checklist de base de datos -->

- [ ] **Migraciones**: Se crearon las migraciones necesarias
- [ ] **Rollback**: Las migraciones se pueden revertir
- [ ] **Datos**: Se preservan los datos existentes
- [ ] **Índices**: Se agregaron índices si es necesario
- [ ] **Constraints**: Se agregaron constraints si es necesario
- [ ] **Seed Data**: Se agregaron datos de prueba si es necesario

## 🐳 Docker
<!-- Checklist de Docker (si aplica) -->

- [ ] **Dockerfile**: El Dockerfile funciona correctamente
- [ ] **Docker Compose**: El docker-compose.yml funciona
- [ ] **Variables**: Las variables de entorno están configuradas
- [ ] **Health Checks**: Los health checks funcionan
- [ ] **Volúmenes**: Los volúmenes están configurados correctamente
- [ ] **Red**: La red interna funciona correctamente

## 📈 Performance
<!-- Checklist de performance -->

- [ ] **Consultas**: Las consultas a la base de datos son eficientes
- [ ] **Caché**: Se implementó caché donde es necesario
- [ ] **Async/Await**: Se usan operaciones asíncronas correctamente
- [ ] **Memory**: No hay memory leaks
- [ ] **CPU**: El uso de CPU es eficiente

## 🔄 Refactoring
<!-- Si es un refactoring, describe los cambios -->

- [ ] No es un refactoring
- [ ] Se mejoró la legibilidad del código
- [ ] Se eliminó código duplicado
- [ ] Se mejoró la estructura del proyecto
- [ ] Se optimizaron las dependencias
- [ ] Se mejoró la separación de responsabilidades

## 📝 Notas adicionales
<!-- Cualquier información adicional que consideres relevante -->

## 🖼️ Screenshots
<!-- Si aplica, agrega screenshots de los cambios visuales -->

## 📋 Checklist final
<!-- Marca con [x] cuando hayas completado cada item -->

- [ ] He leído y entendido el código de conducta del proyecto
- [ ] He realizado una auto-revisión de mi código
- [ ] He comentado mi código, especialmente en áreas difíciles de entender
- [ ] He realizado los cambios correspondientes en la documentación
- [ ] Mis cambios no generan nuevas advertencias
- [ ] He agregado tests que prueban que mi fix es efectivo o que mi feature funciona
- [ ] Los tests nuevos y existentes pasan localmente con mis cambios
- [ ] Cualquier cambio dependiente ha sido mergeado y publicado

## 🚀 Deployment
<!-- Información sobre el deployment -->

- [ ] No requiere deployment especial
- [ ] Requiere migraciones de base de datos
- [ ] Requiere cambios en variables de entorno
- [ ] Requiere cambios en configuración de Docker
- [ ] Requiere cambios en CI/CD

## 🔗 Issues relacionados
<!-- Lista los issues relacionados con este PR -->

- Closes #
- Fixes #
- Related to #

## 👥 Revisores
<!-- Menciona a las personas que deberían revisar este PR -->

@username1 @username2

---

**Nota**: Por favor, asegúrate de que todos los checks de CI pasen antes de solicitar la revisión.
