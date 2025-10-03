using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelRequests.Infrastructure.Persistence;

namespace TravelRequests.Api.Controllers;

/// <summary>
/// Health check controller for Docker and monitoring
/// </summary>
[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(AppDbContext context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Basic health check endpoint
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Service = "Travel Requests API",
            Version = "1.0.0"
        });
    }

    /// <summary>
    /// Detailed health check including database connectivity
    /// </summary>
    /// <returns>Detailed health status</returns>
    [HttpGet("detailed")]
    public async Task<IActionResult> GetDetailed()
    {
        var healthStatus = new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Service = "Travel Requests API",
            Version = "1.0.0",
            Checks = new Dictionary<string, object>()
        };

        try
        {
            // Check database connectivity
            var canConnect = await _context.Database.CanConnectAsync();
            healthStatus.Checks["database"] = new
            {
                Status = canConnect ? "Healthy" : "Unhealthy",
                Message = canConnect ? "Database connection successful" : "Database connection failed"
            };

            if (!canConnect)
            {
                healthStatus = healthStatus with { Status = "Unhealthy" };
            }

            // Check if we can execute a simple query
            if (canConnect)
            {
                try
                {
                    var userCount = await _context.Users.CountAsync();
                    healthStatus.Checks["database_query"] = new
                    {
                        Status = "Healthy",
                        Message = $"Database query successful. Users count: {userCount}"
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Database query failed");
                    healthStatus.Checks["database_query"] = new
                    {
                        Status = "Unhealthy",
                        Message = $"Database query failed: {ex.Message}"
                    };
                    healthStatus = healthStatus with { Status = "Unhealthy" };
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            healthStatus = healthStatus with { Status = "Unhealthy" };
            healthStatus.Checks["database"] = new
            {
                Status = "Unhealthy",
                Message = $"Health check failed: {ex.Message}"
            };
        }

        var statusCode = healthStatus.Status == "Healthy" ? 200 : 503;
        return StatusCode(statusCode, healthStatus);
    }

    /// <summary>
    /// Readiness check for Kubernetes/Docker
    /// </summary>
    /// <returns>Readiness status</returns>
    [HttpGet("ready")]
    public async Task<IActionResult> GetReady()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            if (canConnect)
            {
                return Ok(new { Status = "Ready", Timestamp = DateTime.UtcNow });
            }
            else
            {
                return StatusCode(503, new { Status = "Not Ready", Timestamp = DateTime.UtcNow });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Readiness check failed");
            return StatusCode(503, new { Status = "Not Ready", Error = ex.Message, Timestamp = DateTime.UtcNow });
        }
    }

    /// <summary>
    /// Liveness check for Kubernetes/Docker
    /// </summary>
    /// <returns>Liveness status</returns>
    [HttpGet("live")]
    public IActionResult GetLive()
    {
        return Ok(new { Status = "Alive", Timestamp = DateTime.UtcNow });
    }
}
