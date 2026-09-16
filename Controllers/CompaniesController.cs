using Funeral_Management_Backend.Data;
using Funeral_Management.DTOs.Company;
using Funeral_Management_Backend.Services.CurrentUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CompaniesController(AppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var company = await _context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (company == null)
            return NotFound();

        return Ok(new CompanyResponseDto
        {
            Id = company.Id,
            Name = company.Name,
            Street = company.Street,
            PostalCode = company.PostalCode,
            City = company.City,
            Country = company.Country,
            Phone = company.Phone,
            Email = company.Email,
            CreatedAt = company.CreatedAt
        });
    }
}