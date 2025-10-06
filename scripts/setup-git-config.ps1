# Script para configurar Git con mejores prácticas
# Ejecutar: .\scripts\setup-git-config.ps1

Write-Host "🔧 Configurando Git con mejores prácticas..." -ForegroundColor Green

# Configuraciones básicas
Write-Host "📝 Configurando configuraciones básicas..." -ForegroundColor Cyan

# Configurar colores
git config --global color.ui auto
git config --global color.branch auto
git config --global color.diff auto
git config --global color.status auto

# Configurar editor (VS Code)
git config --global core.editor "code --wait"

# Configurar merge tool
git config --global merge.tool vscode
git config --global mergetool.vscode.cmd "code --wait $MERGED"

# Configurar diff tool
git config --global diff.tool vscode
git config --global difftool.vscode.cmd "code --wait --diff $LOCAL $REMOTE"

# Configuraciones de seguridad
Write-Host "🔒 Configurando medidas de seguridad..." -ForegroundColor Cyan

# Prevenir push accidental a main
git config --global push.default simple

# Configurar pull strategy
git config --global pull.rebase false

# Configurar autocrlf para Windows
git config --global core.autocrlf true

# Configurar ignorecase
git config --global core.ignorecase true

# Configuraciones de alias útiles
Write-Host "⚡ Configurando alias útiles..." -ForegroundColor Cyan

# Alias básicos
git config --global alias.st status
git config --global alias.co checkout
git config --global alias.br branch
git config --global alias.ci commit
git config --global alias.unstage "reset HEAD --"

# Alias avanzados
git config --global alias.last "log -1 HEAD"
git config --global alias.visual "!gitk"
git config --global alias.lg "log --oneline --decorate --graph --all"
git config --global alias.undo "reset --soft HEAD~1"
git config --global alias.amend "commit --amend --no-edit"

# Alias para el proyecto
git config --global alias.cleanup "!git branch --merged | grep -v '\\*\\|main\\|master' | xargs -n 1 git branch -d"
git config --global alias.fresh "!git checkout main && git pull && git branch -D"
git config --global alias.wip "commit -am 'WIP: work in progress'"
git config --global alias.unwip "reset HEAD~1"

# Configuraciones de commit
Write-Host "📋 Configurando plantillas de commit..." -ForegroundColor Cyan

# Crear plantilla de commit
$commitTemplate = @"
# <tipo>(<scope>): <descripción>
#
# <descripción detallada del cambio>
#
# <tipo> puede ser:
#   feat:     Nueva funcionalidad
#   fix:      Corrección de bug
#   docs:     Cambios en documentación
#   style:    Cambios de formato (espacios, comas, etc.)
#   refactor: Refactoring de código
#   test:     Agregar o modificar tests
#   chore:    Cambios en herramientas, configuración, etc.
#   perf:     Mejoras de rendimiento
#   ci:       Cambios en CI/CD
#   build:    Cambios en sistema de build
#   revert:   Revertir commit anterior
#
# <scope> es opcional y puede ser:
#   auth, api, db, docker, tests, docs, etc.
#
# Ejemplos:
#   feat(auth): add JWT authentication
#   fix(api): resolve validation error
#   docs: update README with setup instructions
#
# ---
# Si tu commit cierra un issue, usa:
# Closes #123
# Fixes #456
# Related to #789
"@

$templatePath = "$env:USERPROFILE\.gitmessage"
$commitTemplate | Out-File -FilePath $templatePath -Encoding UTF8
git config --global commit.template $templatePath

Write-Host "✅ Configuración de Git completada" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Configuraciones aplicadas:" -ForegroundColor Cyan
Write-Host "  • Colores habilitados" -ForegroundColor White
Write-Host "  • Editor configurado (VS Code)" -ForegroundColor White
Write-Host "  • Herramientas de merge/diff configuradas" -ForegroundColor White
Write-Host "  • Alias útiles creados" -ForegroundColor White
Write-Host "  • Plantilla de commit configurada" -ForegroundColor White
Write-Host "  • Medidas de seguridad aplicadas" -ForegroundColor White
Write-Host ""
Write-Host "💡 Usa 'git <alias>' para usar los nuevos comandos" -ForegroundColor Yellow
Write-Host "💡 Los commits ahora abrirán VS Code con la plantilla" -ForegroundColor Yellow
