using Funeral_Management_Backend.Data;
using Funeral_Management.DTOs.CaseLocation;
using Funeral_Management_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management.Controllers;

[ApiController]
[Route("api/cases/{caseId:int}/locations")]
public class CaseLocationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CaseLocationsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        int caseId,
        AddCaseLocationDto dto)
    {
        var caseExists = await _context.Cases
            .AnyAsync(c => c.Id == caseId);

        if (!caseExists)
            return NotFound("Case not found.");

        var locationExists = await _context.Locations
            .AnyAsync(l => l.Id == dto.LocationId);

        if (!locationExists)
            return NotFound("Location not found.");

        var relation = new CaseLocation
        {
            CaseId = caseId,
            LocationId = dto.LocationId,
            Role = dto.Role,
            Notes = dto.Notes
        };

        _context.CaseLocations.Add(relation);
        await _context.SaveChangesAsync();

        return Ok(relation);
    }
}