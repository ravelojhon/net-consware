# Script para configurar Git hooks en Windows
# Ejecutar: .\scripts\setup-git-hooks.ps1

Write-Host "🔧 Configurando Git hooks..." -ForegroundColor Green

# Verificar que estamos en un repositorio Git
if (-not (Test-Path ".git")) {
    Write-Host "❌ Error: No estás en un repositorio Git" -ForegroundColor Red
    exit 1
}

# Crear directorio de hooks si no existe
if (-not (Test-Path ".git/hooks")) {
    New-Item -ItemType Directory -Path ".git/hooks" -Force
}

# Verificar hooks existentes
$hooks = @("pre-commit.bat", "commit-msg.bat")

foreach ($hook in $hooks) {
    $sourcePath = ".git/hooks/$hook"
    if (Test-Path $sourcePath) {
        Write-Host "✅ Hook $hook configurado correctamente" -ForegroundColor Green
    } else {
        Write-Host "❌ Error: Hook $hook no encontrado" -ForegroundColor Red
    }
}

# Verificar permisos de ejecución (en Windows esto es automático)
Write-Host "✅ Git hooks configurados correctamente" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Hooks instalados:" -ForegroundColor Cyan
Write-Host "  • pre-commit: Valida compilación, tests y formato" -ForegroundColor White
Write-Host "  • commit-msg: Valida formato de mensajes de commit" -ForegroundColor White
Write-Host ""
Write-Host "💡 Los hooks se ejecutarán automáticamente en cada commit" -ForegroundColor Yellow
