using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelRequests.Application.Contracts.Services;
using TravelRequests.Application.DTOs.TravelRequest;
using TravelRequests.Domain.Enums;

namespace TravelRequests.Api.Controllers;

/// <summary>
/// Controlador para gestión de solicitudes de viaje
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TravelRequestsController : ControllerBase
{
    private readonly ITravelRequestService _travelRequestService;

    public TravelRequestsController(ITravelRequestService travelRequestService)
    {
        _travelRequestService = travelRequestService;
    }

    /// <summary>
    /// Obtiene las solicitudes de viaje del usuario autenticado o todas si es aprobador
    /// </summary>
    /// <param name="all">Si es true y el usuario es aprobador, devuelve todas las solicitudes</param>
    /// <param name="skip">Número de registros a omitir (paginación)</param>
    /// <param name="take">Número de registros a tomar (paginación)</param>
    /// <returns>Lista de solicitudes de viaje</returns>
    /// <response code="200">Lista de solicitudes obtenida exitosamente</response>
    /// <response code="401">Token JWT inválido o expirado</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TravelRequestListResponse>), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<IEnumerable<TravelRequestListResponse>>> GetTravelRequests(
        [FromQuery] bool all = false,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10)
    {
        try
        {
            var userId = GetCurrentUserId();
            var isApprover = IsApprover();

            IEnumerable<TravelRequestListResponse> requests;

            if (all && isApprover)
            {
                // Aprobadores pueden ver todas las solicitudes
                requests = await _travelRequestService.GetAllAsync(skip, take);
            }
            else
            {
                // Usuarios normales ven solo sus solicitudes
                requests = await _travelRequestService.GetByUserIdAsync(userId, skip, take);
            }

            return Ok(requests);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching travel requests", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TravelRequestResponse>> GetTravelRequest(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var isApprover = IsApprover();

            var request = await _travelRequestService.GetByIdAsync(id, userId, isApprover);
            if (request == null)
            {
                return NotFound(new { message = "Travel request not found" });
            }

            return Ok(request);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching travel request", error = ex.Message });
        }
    }

    /// <summary>
    /// Crea una nueva solicitud de viaje
    /// </summary>
    /// <param name="request">Datos de la solicitud de viaje</param>
    /// <returns>Solicitud de viaje creada</returns>
    /// <response code="201">Solicitud creada exitosamente</response>
    /// <response code="400">Datos de entrada inválidos</response>
    /// <response code="401">Token JWT inválido o expirado</response>
    [HttpPost]
    [ProducesResponseType(typeof(TravelRequestResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<TravelRequestResponse>> CreateTravelRequest([FromBody] CreateTravelRequestRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var travelRequest = await _travelRequestService.CreateAsync(request, userId);

            return CreatedAtAction(nameof(GetTravelRequest), new { id = travelRequest.Id }, travelRequest);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating travel request", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TravelRequestResponse>> UpdateTravelRequest(Guid id, [FromBody] UpdateTravelRequestRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var isApprover = IsApprover();

            var updatedRequest = await _travelRequestService.UpdateAsync(id, request, userId, isApprover);
            return Ok(updatedRequest);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating travel request", error = ex.Message });
        }
    }

    /// <summary>
    /// Cambia el estado de una solicitud de viaje (solo para aprobadores)
    /// </summary>
    /// <param name="id">ID de la solicitud de viaje</param>
    /// <param name="request">Nuevo estado y razón de rechazo (si aplica)</param>
    /// <returns>Solicitud de viaje actualizada</returns>
    /// <response code="200">Estado cambiado exitosamente</response>
    /// <response code="400">Datos de entrada inválidos</response>
    /// <response code="401">Token JWT inválido o expirado</response>
    /// <response code="403">Usuario no tiene permisos de aprobador</response>
    /// <response code="404">Solicitud no encontrada</response>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(TravelRequestResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<TravelRequestResponse>> ChangeStatus(Guid id, [FromBody] ChangeStatusRequest request)
    {
        try
        {
            // Solo aprobadores pueden cambiar estado
            if (!IsApprover())
            {
                return Forbid("Only approvers can change travel request status");
            }

            var approverId = GetCurrentUserId();
            var updatedRequest = await _travelRequestService.ChangeStatusAsync(id, request, approverId);

            return Ok(updatedRequest);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while changing travel request status", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTravelRequest(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var isApprover = IsApprover();

            var deleted = await _travelRequestService.DeleteAsync(id, userId, isApprover);
            if (!deleted)
            {
                return NotFound(new { message = "Travel request not found" });
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting travel request", error = ex.Message });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub") ?? User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user token");
        }
        return userId;
    }

    private bool IsApprover()
    {
        var roleClaim = User.FindFirst("role") ?? User.FindFirst(ClaimTypes.Role);
        if (roleClaim == null)
        {
            return false;
        }

        return roleClaim.Value == "Manager" || roleClaim.Value == "Admin";
    }
}
