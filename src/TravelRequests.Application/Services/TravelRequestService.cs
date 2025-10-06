using TravelRequests.Application.Contracts.Repositories;
using TravelRequests.Application.Contracts.Services;
using TravelRequests.Application.DTOs.TravelRequest;
using TravelRequests.Domain.Entities;
using TravelRequests.Domain.Enums;

namespace TravelRequests.Application.Services;

public class TravelRequestService : ITravelRequestService
{
    private readonly ITravelRequestRepository _travelRequestRepository;
    private readonly IUserRepository _userRepository;

    public TravelRequestService(
        ITravelRequestRepository travelRequestRepository,
        IUserRepository userRepository)
    {
        _travelRequestRepository = travelRequestRepository;
        _userRepository = userRepository;
    }

    public async Task<TravelRequestResponse> CreateAsync(CreateTravelRequestRequest request, Guid userId)
    {
        // Validaciones de negocio
        ValidateTravelRequest(request);

        // Crear solicitud de viaje
        var travelRequest = new TravelRequest(
            userId,
            request.OriginCity,
            request.DestinationCity,
            request.DateFrom,
            request.DateTo,
            request.Justification
        );

        await _travelRequestRepository.AddAsync(travelRequest);
        return await MapToResponseAsync(travelRequest);
    }

    public async Task<TravelRequestResponse?> GetByIdAsync(Guid id, Guid userId, bool isApprover = false)
    {
        var travelRequest = await _travelRequestRepository.GetWithUserAsync(id);
        if (travelRequest == null)
        {
            return null;
        }

        // Verificar permisos
        if (!isApprover && travelRequest.UserId != userId)
        {
            throw new UnauthorizedAccessException("You don't have permission to view this travel request");
        }

        return await MapToResponseAsync(travelRequest);
    }

    public async Task<IEnumerable<TravelRequestListResponse>> GetByUserIdAsync(Guid userId, int skip = 0, int take = 10)
    {
        var travelRequests = await _travelRequestRepository.GetByUserIdAsync(userId, skip, take);
        return travelRequests.Select(MapToListResponse);
    }

    public async Task<IEnumerable<TravelRequestListResponse>> GetAllAsync(int skip = 0, int take = 10)
    {
        var travelRequests = await _travelRequestRepository.GetWithUsersAsync(skip, take);
        return travelRequests.Select(MapToListResponse);
    }

    public async Task<TravelRequestResponse> UpdateAsync(Guid id, UpdateTravelRequestRequest request, Guid userId, bool isApprover = false)
    {
        var travelRequest = await _travelRequestRepository.GetByIdAsync(id);
        if (travelRequest == null)
        {
            throw new InvalidOperationException("Travel request not found");
        }

        // Verificar permisos
        if (!isApprover && travelRequest.UserId != userId)
        {
            throw new UnauthorizedAccessException("You don't have permission to update this travel request");
        }

        // Verificar que no esté aprobada o rechazada
        if (travelRequest.Status != TravelRequestStatus.Pending)
        {
            throw new InvalidOperationException("Cannot update travel request that has been approved or rejected");
        }

        // Validaciones de negocio
        ValidateTravelRequest(request);

        // Actualizar solicitud
        travelRequest.UpdateDetails(
            request.OriginCity,
            request.DestinationCity,
            request.DateFrom,
            request.DateTo,
            request.Justification
        );

        await _travelRequestRepository.UpdateAsync(travelRequest);
        return await MapToResponseAsync(travelRequest);
    }

    public async Task<TravelRequestResponse> ChangeStatusAsync(Guid id, ChangeStatusRequest request, Guid approverId)
    {
        var travelRequest = await _travelRequestRepository.GetWithUserAsync(id);
        if (travelRequest == null)
        {
            throw new InvalidOperationException("Travel request not found");
        }

        // Verificar que solo esté pendiente
        if (travelRequest.Status != TravelRequestStatus.Pending)
        {
            throw new InvalidOperationException("Can only change status of pending travel requests");
        }

        // Cambiar estado
        if (request.Status == TravelRequestStatus.Approved)
        {
            travelRequest.Approve(approverId);
        }
        else if (request.Status == TravelRequestStatus.Rejected)
        {
            if (string.IsNullOrWhiteSpace(request.RejectionReason))
            {
                throw new ArgumentException("Rejection reason is required when rejecting a travel request");
            }

            travelRequest.Reject(request.RejectionReason, approverId);
        }
        else
        {
            throw new ArgumentException("Invalid status change");
        }

        await _travelRequestRepository.UpdateAsync(travelRequest);
        return await MapToResponseAsync(travelRequest);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId, bool isApprover = false)
    {
        var travelRequest = await _travelRequestRepository.GetByIdAsync(id);
        if (travelRequest == null)
        {
            return false;
        }

        // Verificar permisos
        if (!isApprover && travelRequest.UserId != userId)
        {
            throw new UnauthorizedAccessException("You don't have permission to delete this travel request");
        }

        // Verificar que esté pendiente
        if (travelRequest.Status != TravelRequestStatus.Pending)
        {
            throw new InvalidOperationException("Cannot delete travel request that has been approved or rejected");
        }

        await _travelRequestRepository.DeleteAsync(travelRequest);
        return true;
    }

    private static void ValidateTravelRequest(CreateTravelRequestRequest request)
    {
        ValidateTravelRequest(request.OriginCity, request.DestinationCity, request.DateFrom, request.DateTo);
    }

    private static void ValidateTravelRequest(UpdateTravelRequestRequest request)
    {
        ValidateTravelRequest(request.OriginCity, request.DestinationCity, request.DateFrom, request.DateTo);
    }

    private static void ValidateTravelRequest(string originCity, string destinationCity, DateTime dateFrom, DateTime dateTo)
    {
        // Validar que las fechas sean válidas
        if (dateFrom >= dateTo)
        {
            throw new ArgumentException("End date must be after start date");
        }

        // Validar que no sea en el pasado
        if (dateFrom < DateTime.Today)
        {
            throw new ArgumentException("Travel dates cannot be in the past");
        }

        // Validar que origen y destino sean diferentes
        if (string.Equals(originCity, destinationCity, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Origin and destination cities must be different");
        }
    }

    private async Task<TravelRequestResponse> MapToResponseAsync(TravelRequest travelRequest)
    {
        var user = await _userRepository.GetByIdAsync(travelRequest.UserId);
        var approver = travelRequest.ApprovedBy.HasValue
            ? await _userRepository.GetByIdAsync(travelRequest.ApprovedBy.Value)
            : null;

        return new TravelRequestResponse
        {
            Id = travelRequest.Id,
            UserId = travelRequest.UserId,
            UserName = user?.Name ?? "Unknown",
            UserEmail = user?.Email ?? "Unknown",
            OriginCity = travelRequest.OriginCity,
            DestinationCity = travelRequest.DestinationCity,
            DateFrom = travelRequest.DateFrom,
            DateTo = travelRequest.DateTo,
            Justification = travelRequest.Justification,
            Status = travelRequest.Status,
            RejectionReason = travelRequest.RejectionReason,
            ApprovedBy = travelRequest.ApprovedBy,
            ApproverName = approver?.Name,
            ApprovedAt = travelRequest.ApprovedAt,
            CreatedAt = travelRequest.CreatedAt,
            UpdatedAt = travelRequest.UpdatedAt
        };
    }

    private static TravelRequestListResponse MapToListResponse(TravelRequest travelRequest)
    {
        return new TravelRequestListResponse
        {
            Id = travelRequest.Id,
            UserId = travelRequest.UserId,
            UserName = "Unknown", // Se llenará en el controlador si es necesario
            OriginCity = travelRequest.OriginCity,
            DestinationCity = travelRequest.DestinationCity,
            DateFrom = travelRequest.DateFrom,
            DateTo = travelRequest.DateTo,
            Status = travelRequest.Status,
            CreatedAt = travelRequest.CreatedAt
        };
    }
}
