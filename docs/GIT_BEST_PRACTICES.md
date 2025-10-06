# 🚀 Mejores Prácticas de Git

Este documento describe las mejores prácticas de Git implementadas en el proyecto TravelRequests API.

## 📋 Tabla de Contenidos

- [Configuración Inicial](#configuración-inicial)
- [Flujo de Trabajo](#flujo-de-trabajo)
- [Convenciones de Commits](#convenciones-de-commits)
- [Git Hooks](#git-hooks)
- [Seguridad](#seguridad)
- [Alias Útiles](#alias-útiles)
- [Troubleshooting](#troubleshooting)

## 🔧 Configuración Inicial

### 1. Configurar Git Globalmente

```powershell
# Ejecutar script de configuración
.\scripts\setup-git-config.ps1
```

Este script configura:
- ✅ Colores y editor (VS Code)
- ✅ Herramientas de merge/diff
- ✅ Alias útiles
- ✅ Plantilla de commits
- ✅ Configuraciones de seguridad

### 2. Configurar Medidas de Seguridad

```powershell
# Configurar seguridad
.\scripts\git-security.ps1
```

### 3. Instalar Git Hooks

```powershell
# Instalar hooks automáticos
.\scripts\setup-git-hooks.ps1
```

### 4. Proteger Rama Principal

```powershell
# Proteger rama main
.\scripts\protect-main-branch.ps1
```

## 🌿 Flujo de Trabajo

### Branching Strategy

```
main (protegida)
├── feature/auth-jwt
├── feature/docker
├── fix/validation-error
├── chore/update-dependencies
└── release/v1.0.0
```

### Flujo de Desarrollo

1. **Crear rama feature**
   ```bash
   git checkout main
   git pull origin main
   git checkout -b feature/nueva-funcionalidad
   ```

2. **Desarrollar con commits frecuentes**
   ```bash
   git add .
   git commit -m "feat(auth): add JWT validation"
   ```

3. **Sincronizar con main**
   ```bash
   git checkout main
   git pull origin main
   git checkout feature/nueva-funcionalidad
   git rebase main
   ```

4. **Crear Pull Request**
   - Usar template de PR
   - Solicitar revisión
   - Esperar aprobación

5. **Merge y limpieza**
   ```bash
   git checkout main
   git pull origin main
   git branch -d feature/nueva-funcionalidad
   ```

## 📝 Convenciones de Commits

### Formato

```
<tipo>(<scope>): <descripción>

<descripción detallada>

<footer>
```

### Tipos Válidos

| Tipo | Descripción | Ejemplo |
|------|-------------|---------|
| `feat` | Nueva funcionalidad | `feat(auth): add JWT authentication` |
| `fix` | Corrección de bug | `fix(api): resolve validation error` |
| `docs` | Documentación | `docs: update README with setup` |
| `style` | Formato de código | `style: fix indentation` |
| `refactor` | Refactoring | `refactor(service): improve error handling` |
| `test` | Tests | `test(auth): add unit tests for JWT` |
| `chore` | Mantenimiento | `chore: update dependencies` |
| `perf` | Performance | `perf(db): optimize query performance` |
| `ci` | CI/CD | `ci: add GitHub Actions workflow` |
| `build` | Build system | `build: update Dockerfile` |
| `revert` | Revertir | `revert: revert commit abc123` |

### Scopes Comunes

- `auth` - Autenticación
- `api` - API endpoints
- `db` - Base de datos
- `docker` - Contenedores
- `tests` - Testing
- `docs` - Documentación
- `ci` - CI/CD

### Ejemplos

```bash
# Nueva funcionalidad
git commit -m "feat(auth): add password reset flow"

# Corrección de bug
git commit -m "fix(api): resolve null reference in user service"

# Documentación
git commit -m "docs: add API documentation for travel requests"

# Refactoring
git commit -m "refactor(domain): extract value objects"

# Test
git commit -m "test(auth): add integration tests for JWT"

# Chore
git commit -m "chore: update .NET to version 9.0"
```

## 🪝 Git Hooks

### Pre-commit Hook

Valida automáticamente:
- ✅ Compilación del proyecto
- ✅ Tests unitarios
- ✅ Formato de código

### Commit-msg Hook

Valida:
- ✅ Formato de mensaje de commit
- ✅ Uso de Conventional Commits

### Configurar Hooks

```powershell
# Los hooks se instalan automáticamente con:
.\scripts\setup-git-hooks.ps1
```

## 🔒 Seguridad

### Medidas Implementadas

1. **Protección de rama main**
   - No se puede hacer push directo
   - Solo mediante Pull Requests

2. **Validación de commits**
   - Formato obligatorio
   - Validación de código

3. **Configuración segura**
   - Autocrlf configurado
   - Ignorecase habilitado
   - Filemode deshabilitado

4. **Firma GPG (opcional)**
   - Commits firmados digitalmente
   - Verificación de autenticidad

### Archivos Sensibles

Los siguientes archivos están en `.gitignore`:
- `.env*` - Variables de entorno
- `*.db` - Bases de datos
- `logs/` - Archivos de log
- `*.log` - Archivos de log

## ⚡ Alias Útiles

### Alias Básicos

```bash
git st          # git status
git co          # git checkout
git br          # git branch
git ci          # git commit
git unstage     # git reset HEAD --
```

### Alias Avanzados

```bash
git lg          # Log con gráfico
git last        # Último commit
git visual      # Gitk
git undo        # Reset --soft HEAD~1
git amend       # Commit --amend --no-edit
```

### Alias del Proyecto

```bash
git cleanup     # Limpiar ramas merged
git fresh       # Actualizar desde main
git wip         # Commit temporal
git unwip       # Deshacer commit temporal
```

## 🐛 Troubleshooting

### Problemas Comunes

#### 1. Hook no se ejecuta

```powershell
# Verificar permisos
Get-ChildItem .git/hooks/*.bat

# Reinstalar hooks
.\scripts\setup-git-hooks.ps1
```

#### 2. Commit falla por formato

```bash
# Ver formato correcto
git commit -m "feat(auth): add new feature"

# Editar último commit
git commit --amend -m "feat(auth): add new feature"
```

#### 3. Push a main bloqueado

```bash
# Crear rama feature
git checkout -b feature/tu-cambio

# Hacer commit
git add .
git commit -m "feat: your changes"

# Push a rama feature
git push origin feature/tu-cambio

# Crear Pull Request
```

#### 4. Merge conflicts

```bash
# Actualizar main
git checkout main
git pull origin main

# Rebase feature branch
git checkout feature/tu-cambio
git rebase main

# Resolver conflicts y continuar
git add .
git rebase --continue
```

### Comandos de Emergencia

```bash
# Deshacer último commit (mantener cambios)
git reset --soft HEAD~1

# Deshacer último commit (perder cambios)
git reset --hard HEAD~1

# Deshacer merge
git reset --hard ORIG_HEAD

# Limpiar archivos no trackeados
git clean -fd

# Ver historial de cambios
git reflog
```

## 📚 Recursos Adicionales

- [Conventional Commits](https://www.conventionalcommits.org/)
- [Git Flow](https://nvie.com/posts/a-successful-git-branching-model/)
- [Git Hooks](https://git-scm.com/book/en/v2/Customizing-Git-Git-Hooks)
- [Git Security](https://git-scm.com/book/en/v2/Git-Tools-Signing-Your-Work)

## 🤝 Contribuir

Si encuentras problemas o tienes sugerencias:

1. Crea un issue
2. Propón mejoras en un PR
3. Actualiza esta documentación

---

**Nota**: Este documento se actualiza regularmente. Mantén tu fork sincronizado.
