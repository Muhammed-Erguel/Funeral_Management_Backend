using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.DTOs.Auth;
using Funeral_Management_Backend.Models;
using Funeral_Management_Backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Funeral_Management_Backend.Services.Auth;

namespace Funeral_Management.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly TokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthController(
        AppDbContext context,
        TokenService tokenService,
        IRefreshTokenService refreshTokenService)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        // 1. E-Mail normalisieren
        var email = dto.Email.Trim().ToLowerInvariant();

        // 2. Prüfen, ob E-Mail schon existiert
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == email);

        if (emailExists)
        {
            return Conflict(new
            {
                message = "A user with this email already exists."
            });
        }

        // 3. Firma erstellen
        var company = new Company
        {
            Name = dto.Company.Name,
            Street = dto.Company.Street,
            PostalCode = dto.Company.PostalCode,
            City = dto.Company.City,
            Country = dto.Company.Country,
            Phone = dto.Company.Phone,
            Email = dto.Company.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // 4. User erstellen
        var user = new User
        {
            Email = email,

            // Erster Benutzer der Firma ist Owner
            Role = "Owner",

            Company = company,

            CreatedAt = DateTime.UtcNow
        };

        // 5. Passwort hashen
        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            dto.Password
        );

        // 6. User + Company speichern
        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        // 7. Access Token erstellen
        var accessToken = _tokenService.CreateAccessToken(user);

        var refreshToken = await _refreshTokenService.CreateAsync(user);

        // 8. Response
        return Created("", new
        {
            accessToken,
            refreshToken
        });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            dto.Password
        );

        if (result == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        var accessToken = _tokenService.CreateAccessToken(user);

        var refreshToken = await _refreshTokenService.CreateAsync(user);

        return Ok(new
        {
            accessToken,
            refreshToken
        });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenDto dto)
    {
        var user = await _refreshTokenService
            .ValidateAsync(dto.RefreshToken);

        if (user == null)
        {
            return Unauthorized(new
            {
                message = "Invalid or expired refresh token."
            });
        }

        // Alten Refresh Token ungültig machen
        await _refreshTokenService
            .RevokeAsync(dto.RefreshToken);

        // Neuen Access Token erzeugen
        var accessToken =
            _tokenService.CreateAccessToken(user);

        // Neuen Refresh Token erzeugen
        var newRefreshToken =
            await _refreshTokenService.CreateAsync(user);

        return Ok(new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken
        });
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenDto dto)
    {
        await _refreshTokenService.RevokeAsync(dto.RefreshToken);

        return NoContent();
    }
}