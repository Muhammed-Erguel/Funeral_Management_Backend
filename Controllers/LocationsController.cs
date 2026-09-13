using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.DTOs.Location;
using Funeral_Management_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management_Backend.Controllers;

[ApiController]
[Route("api/companies/{companyId:int}/locations")]
public class LocationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public LocationsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int companyId)
    {
        var locations = await _context.Locations
            .AsNoTracking()
            .Where(l => l.CompanyId == companyId)
            .ToListAsync();

        return Ok(locations);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        int companyId,
        CreateLocationDto dto)
    {
        var location = new Location
        {
            CompanyId = companyId,
            Name = dto.Name,
            Type = dto.Type,
            Street = dto.Street,
            PostalCode = dto.PostalCode,
            City = dto.City,
            Country = dto.Country,
            Phone = dto.Phone,
            Email = dto.Email,
            OpeningHours = dto.OpeningHours,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Locations.Add(location);
        await _context.SaveChangesAsync();

        return Ok(location);
    }
}