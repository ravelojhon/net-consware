using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TravelRequests.Application.Contracts.Repositories;
using TravelRequests.Application.Contracts.Services;
using TravelRequests.Application.DTOs.Auth;
using TravelRequests.Domain.Entities;
using TravelRequests.Domain.Enums;

namespace TravelRequests.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly string _jwtKey;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly int _jwtExpirationMinutes;

    public AuthService(
        IUserRepository userRepository,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
        _jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
        _jwtAudience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");
        _jwtExpirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Verificar si el email ya existe
        if (await _userRepository.ExistsByEmailAsync(request.Email))
        {
            throw new InvalidOperationException("Email already exists");
        }

        // Hash de la contraseña
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Crear usuario
        var user = new User(request.Name, request.Email, passwordHash, request.Role);
        var createdUser = await _userRepository.AddAsync(user);

        // Generar token
        var token = await GenerateTokenAsync(createdUser.Id, createdUser.Email, createdUser.Role.ToString());
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes);

        return new AuthResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = createdUser.Id.ToString(),
            UserName = createdUser.Name,
            UserEmail = createdUser.Email,
            Role = createdUser.Role.ToString()
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // Buscar usuario por email
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Verificar contraseña
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Generar token
        var token = await GenerateTokenAsync(user.Id, user.Email, user.Role.ToString());
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes);

        return new AuthResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.Id.ToString(),
            UserName = user.Name,
            UserEmail = user.Email,
            Role = user.Role.ToString()
        };
    }

    public async Task<UserResponse?> GetUserByIdAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtIssuer,
                ValidateAudience = true,
                ValidAudience = _jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string> GenerateTokenAsync(Guid userId, string email, string role)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, role),
            new("sub", userId.ToString()),
            new("email", email),
            new("role", role)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
            Issuer = _jwtIssuer,
            Audience = _jwtAudience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
