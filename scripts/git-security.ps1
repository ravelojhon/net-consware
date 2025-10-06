# Script para configurar medidas de seguridad en Git
# Ejecutar: .\scripts\git-security.ps1

Write-Host "🔒 Configurando medidas de seguridad en Git..." -ForegroundColor Green

# Verificar configuración actual
Write-Host "📋 Verificando configuración actual..." -ForegroundColor Cyan

$currentUser = git config --global user.name
$currentEmail = git config --global user.email

Write-Host "Usuario actual: $currentUser" -ForegroundColor White
Write-Host "Email actual: $currentEmail" -ForegroundColor White

# Configuraciones de seguridad
Write-Host "🛡️ Aplicando configuraciones de seguridad..." -ForegroundColor Cyan

# Prevenir push accidental a ramas protegidas
git config --global push.default simple

# Configurar pull strategy para evitar merge commits innecesarios
git config --global pull.rebase false

# Configurar autocrlf para Windows (evitar problemas de line endings)
git config --global core.autocrlf true

# Configurar ignorecase para evitar problemas de case sensitivity
git config --global core.ignorecase true

# Configurar filemode para Windows
git config --global core.filemode false

# Configurar GPG signing (opcional)
Write-Host "🔐 Configuración de firma GPG (opcional)..." -ForegroundColor Cyan
Write-Host "¿Quieres configurar firma GPG para commits? (y/N): " -NoNewline -ForegroundColor Yellow
$gpgChoice = Read-Host

if ($gpgChoice -eq "y" -or $gpgChoice -eq "Y") {
    Write-Host "Configurando firma GPG..." -ForegroundColor Cyan
    
    # Verificar si GPG está instalado
    try {
        gpg --version | Out-Null
        Write-Host "✅ GPG encontrado" -ForegroundColor Green
        
        # Listar claves GPG disponibles
        Write-Host "Claves GPG disponibles:" -ForegroundColor White
        gpg --list-secret-keys --keyid-format=long
        
        Write-Host "Ingresa el ID de la clave GPG que quieres usar: " -NoNewline -ForegroundColor Yellow
        $gpgKey = Read-Host
        
        git config --global user.signingkey $gpgKey
        git config --global commit.gpgsign true
        git config --global tag.gpgsign true
        
        Write-Host "✅ Firma GPG configurada" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ GPG no encontrado. Instala GPG para usar firma de commits" -ForegroundColor Red
        Write-Host "Puedes instalar GPG desde: https://gnupg.org/download/" -ForegroundColor Yellow
    }
}

# Configurar protección de ramas
Write-Host "🌿 Configurando protección de ramas..." -ForegroundColor Cyan

# Crear script para proteger rama main
$protectMainScript = @"
# Script para proteger la rama main
# Ejecutar: .\scripts\protect-main-branch.ps1

Write-Host "🛡️ Configurando protección de rama main..." -ForegroundColor Green

# Verificar si estamos en un repositorio Git
if (-not (Test-Path ".git")) {
    Write-Host "❌ Error: No estás en un repositorio Git" -ForegroundColor Red
    exit 1
}

# Crear archivo de protección de rama
$branchProtection = @"
# Protección de rama main
# Este archivo previene push accidental a main

if [ `$1 = "refs/heads/main" ] || [ `$1 = "refs/heads/master" ]; then
    echo "❌ Error: No puedes hacer push directo a main/master"
    echo "💡 Usa Pull Requests para cambios en main"
    echo "💡 Crea una rama feature: git checkout -b feature/tu-feature"
    exit 1
fi
"@

$branchProtection | Out-File -FilePath ".git/hooks/pre-push" -Encoding UTF8

Write-Host "✅ Protección de rama main configurada" -ForegroundColor Green
Write-Host "💡 Ahora necesitarás usar Pull Requests para cambios en main" -ForegroundColor Yellow
"@

$protectMainScript | Out-File -FilePath "scripts/protect-main-branch.ps1" -Encoding UTF8

# Configurar .gitattributes para manejo de archivos
Write-Host "📁 Configurando .gitattributes..." -ForegroundColor Cyan

$gitattributes = @"
# Configuración de .gitattributes para el proyecto

# Archivos de texto
*.cs text eol=crlf
*.json text eol=crlf
*.md text eol=crlf
*.yml text eol=crlf
*.yaml text eol=crlf
*.xml text eol=crlf
*.ps1 text eol=crlf
*.bat text eol=crlf
*.sh text eol=lf

# Archivos binarios
*.dll binary
*.exe binary
*.pdb binary
*.so binary
*.dylib binary
*.zip binary
*.tar.gz binary
*.rar binary
*.7z binary

# Archivos de configuración sensible
*.env text eol=crlf
*.env.local text eol=crlf
*.env.development text eol=crlf
*.env.production text eol=crlf
*.env.test text eol=crlf

# Archivos de base de datos
*.db binary
*.sqlite binary
*.sqlite3 binary

# Archivos de log
*.log text eol=crlf

# Archivos de Docker
Dockerfile text eol=crlf
docker-compose*.yml text eol=crlf
.dockerignore text eol=crlf

# Archivos de configuración de IDE
.vscode/settings.json text eol=crlf
.vscode/tasks.json text eol=crlf
.vscode/launch.json text eol=crlf
.vscode/extensions.json text eol=crlf
"@

$gitattributes | Out-File -FilePath ".gitattributes" -Encoding UTF8

Write-Host "✅ Configuración de seguridad completada" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Medidas de seguridad aplicadas:" -ForegroundColor Cyan
Write-Host "  • Push default configurado" -ForegroundColor White
Write-Host "  • Configuración de line endings" -ForegroundColor White
Write-Host "  • .gitattributes configurado" -ForegroundColor White
Write-Host "  • Script de protección de rama creado" -ForegroundColor White
if ($gpgChoice -eq "y" -or $gpgChoice -eq "Y") {
    Write-Host "  • Firma GPG configurada" -ForegroundColor White
}
Write-Host ""
Write-Host "💡 Ejecuta '.\scripts\protect-main-branch.ps1' para proteger la rama main" -ForegroundColor Yellow
