using Funeral_Management_Backend.Data;
using Funeral_Management.DTOs.Company;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CompaniesController(AppDbContext context)
    {
        _context = context;
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