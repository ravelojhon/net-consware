using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelRequests.Application.Contracts.Services;
using TravelRequests.Application.DTOs.Auth;

namespace TravelRequests.Api.Controllers;

/// <summary>
/// Controlador para autenticación y gestión de usuarios
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema
    /// </summary>
    /// <param name="request">Datos del usuario a registrar</param>
    /// <returns>Token JWT y información del usuario</returns>
    /// <response code="200">Usuario registrado exitosamente</response>
    /// <response code="400">Datos de entrada inválidos</response>
    /// <response code="409">El email ya está registrado</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during registration", error = ex.Message });
        }
    }

    /// <summary>
    /// Autentica un usuario y genera un token JWT
    /// </summary>
    /// <param name="request">Credenciales de login</param>
    /// <returns>Token JWT y información del usuario</returns>
    /// <response code="200">Login exitoso</response>
    /// <response code="401">Credenciales inválidas</response>
    /// <response code="400">Datos de entrada inválidos</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene la información del usuario autenticado
    /// </summary>
    /// <returns>Información del usuario actual</returns>
    /// <response code="200">Información del usuario obtenida</response>
    /// <response code="401">Token JWT inválido o expirado</response>
    /// <response code="404">Usuario no encontrado</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserResponse), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<UserResponse>> GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirst("sub") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching user data", error = ex.Message });
        }
    }

    [HttpPost("validate-token")]
    public async Task<ActionResult<object>> ValidateToken([FromBody] ValidateTokenRequest request)
    {
        try
        {
            var isValid = await _authService.ValidateTokenAsync(request.Token);
            return Ok(new { isValid });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while validating token", error = ex.Message });
        }
    }

    /// <summary>
    /// Solicita un código de restablecimiento de contraseña
    /// </summary>
    /// <param name="request">Email del usuario</param>
    /// <returns>Código de restablecimiento (simulado para pruebas)</returns>
    /// <response code="200">Código generado exitosamente</response>
    /// <response code="400">Email no encontrado</response>
    [HttpPost("request-password-reset")]
    [ProducesResponseType(typeof(PasswordResetResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<PasswordResetResponse>> RequestPasswordReset([FromBody] RequestPasswordResetRequest request)
    {
        try
        {
            var response = await _authService.RequestPasswordResetAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while requesting password reset", error = ex.Message });
        }
    }

    /// <summary>
    /// Confirma el restablecimiento de contraseña con el código proporcionado
    /// </summary>
    /// <param name="request">Email, código y nueva contraseña</param>
    /// <returns>Confirmación de restablecimiento exitoso</returns>
    /// <response code="200">Contraseña restablecida exitosamente</response>
    /// <response code="400">Código inválido, expirado o ya usado</response>
    [HttpPost("confirm-password-reset")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> ConfirmPasswordReset([FromBody] ConfirmPasswordResetRequest request)
    {
        try
        {
            await _authService.ConfirmPasswordResetAsync(request);
            return Ok(new { message = "Password has been successfully reset" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while confirming password reset", error = ex.Message });
        }
    }
}

public record ValidateTokenRequest(string Token);
