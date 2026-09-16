using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.DTOs.User;
using Funeral_Management_Backend.Models;
using Funeral_Management_Backend.Services.Audit;
using Funeral_Management_Backend.Services.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management_Backend.Controllers;

[Authorize(Roles = "Owner,Admin")]
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogService _auditLog;
    private readonly PasswordHasher<User> _passwordHasher;

    public UsersController(
        AppDbContext context,
        ICurrentUserService currentUser,
        IAuditLogService auditLog)
    {
        _context = context;
        _currentUser = currentUser;
        _auditLog = auditLog;

        _passwordHasher = new PasswordHasher<User>();
    }

    // --------------------------------------------------
    // GET /api/users
    // --------------------------------------------------

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var companyId = _currentUser.CompanyId;

        var users = await _context.Users
            .AsNoTracking()
            .Where(u => u.CompanyId == companyId)
            .Select(u => new UserResponseDto
            {
                Id = u.Id,
                CompanyId = u.CompanyId,
                Email = u.Email,
                Role = u.Role,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();

        return Ok(users);
    }

    // --------------------------------------------------
    // GET /api/users/{id}
    // --------------------------------------------------

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var companyId = _currentUser.CompanyId;

        var user = await _context.Users
            .AsNoTracking()
            .Where(u =>
                u.Id == id &&
                u.CompanyId == companyId)
            .Select(u => new UserResponseDto
            {
                Id = u.Id,
                CompanyId = u.CompanyId,
                Email = u.Email,
                Role = u.Role,
                CreatedAt = u.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    // --------------------------------------------------
    // POST /api/users
    // --------------------------------------------------

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        var companyId = _currentUser.CompanyId;

        var email = dto.Email
            .Trim()
            .ToLowerInvariant();

        // --------------------------------------------------
        // E-Mail prüfen
        // --------------------------------------------------

        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == email);

        if (emailExists)
        {
            return Conflict(new
            {
                message = "Email already exists."
            });
        }

        // --------------------------------------------------
        // Rolle prüfen
        // --------------------------------------------------

        var allowedRoles = new[]
        {
            "Admin",
            "Employee"
        };

        if (!allowedRoles.Contains(dto.Role))
        {
            return BadRequest(new
            {
                message =
                    "Invalid role. Allowed roles are Admin and Employee."
            });
        }

        // --------------------------------------------------
        // User erstellen
        // --------------------------------------------------

        var user = new User
        {
            CompanyId = companyId,
            Email = email,
            Role = dto.Role,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash =
            _passwordHasher.HashPassword(
                user,
                dto.Password
            );

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        // --------------------------------------------------
        // Audit Log
        // --------------------------------------------------

        await _auditLog.LogAsync(
            action: "UserCreated",
            entityType: "User",
            entityId: user.Id,
            caseId: null,
            metadata: new
            {
                user.Email,
                user.Role
            }
        );

        // --------------------------------------------------
        // Response
        // --------------------------------------------------

        var response = new UserResponseDto
        {
            Id = user.Id,
            CompanyId = user.CompanyId,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };

        return CreatedAtAction(
            nameof(GetById),
            new { id = user.Id },
            response
        );
    }
}