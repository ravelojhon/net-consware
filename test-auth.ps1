# Test script para probar endpoints de autenticación

Write-Host "Esperando 5 segundos para que la API inicie..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

$baseUrl = "http://localhost:5090/api"

# Test 1: Registro
Write-Host "`nTest 1: Registrar usuario..." -ForegroundColor Cyan
$registerBody = @{
    name = "Juan Pérez"
    email = "juan@example.com"
    password = "Pass1234!"
    role = 1
} | ConvertTo-Json

try {
    $registerResponse = Invoke-RestMethod -Uri "$baseUrl/auth/register" -Method POST -Body $registerBody -ContentType "application/json"
    Write-Host "✓ Registro exitoso!" -ForegroundColor Green
    Write-Host "Token: $($registerResponse.token.Substring(0, 50))..." -ForegroundColor Gray
    Write-Host "UserId: $($registerResponse.userId)" -ForegroundColor Gray
    Write-Host "Expira: $($registerResponse.expiresAt)" -ForegroundColor Gray
    $token = $registerResponse.token
} catch {
    Write-Host "✗ Error en registro: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Detalles: $responseBody" -ForegroundColor Yellow
    }
}

# Test 2: Login
Write-Host "`nTest 2: Login..." -ForegroundColor Cyan
$loginBody = @{
    email = "juan@example.com"
    password = "Pass1234!"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method POST -Body $loginBody -ContentType "application/json"
    Write-Host "✓ Login exitoso!" -ForegroundColor Green
    Write-Host "Token: $($loginResponse.token.Substring(0, 50))..." -ForegroundColor Gray
    Write-Host "UserId: $($loginResponse.userId)" -ForegroundColor Gray
    $token = $loginResponse.token
} catch {
    Write-Host "✗ Error en login: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Detalles: $responseBody" -ForegroundColor Yellow
    }
}

# Test 3: Obtener usuario actual con token
if ($token) {
    Write-Host "`nTest 3: Obtener usuario actual (con token)..." -ForegroundColor Cyan
    try {
        $headers = @{
            Authorization = "Bearer $token"
        }
        $meResponse = Invoke-RestMethod -Uri "$baseUrl/auth/me" -Method GET -Headers $headers
        Write-Host "✓ Usuario obtenido!" -ForegroundColor Green
        Write-Host "Nombre: $($meResponse.name)" -ForegroundColor Gray
        Write-Host "Email: $($meResponse.email)" -ForegroundColor Gray
        Write-Host "Rol: $($meResponse.role)" -ForegroundColor Gray
    } catch {
        Write-Host "✗ Error al obtener usuario: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`n=== Tests completados ===" -ForegroundColor Green