using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.DTOs.User;
using Funeral_Management_Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management.Controllers;

[ApiController]
[Route("api/companies/{companyId:int}/users")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;

    public UsersController(AppDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int companyId)
    {
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

    [HttpPost]
    public async Task<IActionResult> Create(int companyId, CreateUserDto dto)
    {
        var companyExists = await _context.Companies
            .AnyAsync(c => c.Id == companyId);

        if (!companyExists)
            return NotFound("Company not found.");

        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == dto.Email);

        if (emailExists)
            return Conflict("Email already exists.");

        var user = new User
        {
            CompanyId = companyId,
            Email = dto.Email,
            Role = dto.Role,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash =
            _passwordHasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetAll),
            new { companyId },
            new UserResponseDto
            {
                Id = user.Id,
                CompanyId = user.CompanyId,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            });
    }
}