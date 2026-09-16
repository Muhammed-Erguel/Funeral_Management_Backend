using System.Security.Claims;
using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.DTOs.Me;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management_Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/me")]
public class MeController : ControllerBase
{
    private readonly AppDbContext _context;

    public MeController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetMe()
    {
        var userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        var companyIdValue =
            User.FindFirstValue("company_id");

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized(new
            {
                message = "Invalid user ID."
            });
        }

        if (!int.TryParse(companyIdValue, out var companyId))
        {
            return Unauthorized(new
            {
                message = "Invalid company ID."
            });
        }

        var user = await _context.Users
            .AsNoTracking()
            .Where(u =>
                u.Id == userId &&
                u.CompanyId == companyId)
            .Select(u => new MeResponseDto
            {
                UserId = u.Id,
                Email = u.Email,
                Role = u.Role,

                Company = new CompanyInfoDto
                {
                    Id = u.Company.Id,
                    Name = u.Company.Name
                }
            })
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }

        return Ok(user);
    }
}