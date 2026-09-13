using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.DTOs.Auth;
using Funeral_Management_Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;

    public AuthController(AppDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        // 1. Prüfen, ob E-Mail schon existiert
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == dto.Email);

        if (emailExists)
        {
            return Conflict(new
            {
                message = "A user with this email already exists."
            });
        }

        // 2. Firma erstellen
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

        // 3. User erstellen
        var user = new User
        {
            Email = dto.Email,

            // Erster Benutzer der Firma ist Owner
            Role = "Owner",

            Company = company,

            CreatedAt = DateTime.UtcNow
        };

        // 4. Passwort hashen
        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            dto.Password
        );

        // 5. User hinzufügen
        // Company wird über die Beziehung automatisch mitgespeichert
        _context.Users.Add(user);

        // 6. Datenbank speichern
        await _context.SaveChangesAsync();

        // 7. Response
        return Created("", new
        {
            userId = user.Id,
            email = user.Email,
            role = user.Role,

            company = new
            {
                id = company.Id,
                name = company.Name
            }
        });
    }
}