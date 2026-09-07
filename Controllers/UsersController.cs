using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.Models;
using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        // Prüfen, ob E-Mail bereits existiert
        bool exists = await _context.Users
            .AnyAsync(u => u.Email == dto.Email);

        if (exists)
        {
            return Conflict("Ein Benutzer mit dieser E-Mail existiert bereits.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            Role = dto.Role,
            CreatedAt = DateTime.UtcNow
        };

        // Passwort hashen
        user.PasswordHash =
            _passwordHasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetUser),
            new { id = user.Id },
            new
            {
                user.Id,
                user.Email,
                user.Role,
                user.CreatedAt
            });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _context.Users
            .Where(u => u.Id == id)
            .Select(u => new
            {
                u.Id,
                u.Email,
                u.Role,
                u.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}