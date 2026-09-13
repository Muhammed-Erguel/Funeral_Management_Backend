using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.DTOs.Case;
using Funeral_Management_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management_Backend.Controllers;

[ApiController]
[Route("api/companies/{companyId:int}/cases")]
public class CasesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CasesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int companyId)
    {
        var cases = await _context.Cases
            .AsNoTracking()
            .Where(c => c.CompanyId == companyId)
            .Select(c => new CaseResponseDto
            {
                Id = c.Id,
                CaseNumber = c.CaseNumber,
                Status = c.Status,
                DeceasedFirstName = c.DeceasedFirstName,
                DeceasedLastName = c.DeceasedLastName,
                DateOfBirth = c.DateOfBirth,
                DateOfDeath = c.DateOfDeath,
                DestinationCountry = c.DestinationCountry,
                DestinationCity = c.DestinationCity,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync();

        return Ok(cases);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int companyId, int id)
    {
        var item = await _context.Cases
            .AsNoTracking()
            .Where(c => c.CompanyId == companyId && c.Id == id)
            .Select(c => new CaseResponseDto
            {
                Id = c.Id,
                CaseNumber = c.CaseNumber,
                Status = c.Status,
                DeceasedFirstName = c.DeceasedFirstName,
                DeceasedLastName = c.DeceasedLastName,
                DateOfBirth = c.DateOfBirth,
                DateOfDeath = c.DateOfDeath,
                DestinationCountry = c.DestinationCountry,
                DestinationCity = c.DestinationCity,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (item == null)
            return NotFound();

        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        int companyId,
        int userId,
        CreateCaseDto dto)
    {
        var userExists = await _context.Users
            .AnyAsync(u => u.Id == userId && u.CompanyId == companyId);

        if (!userExists)
            return BadRequest("Invalid user.");

        var year = DateTime.UtcNow.Year;

        var count = await _context.Cases
            .CountAsync(c =>
                c.CompanyId == companyId &&
                c.CreatedAt.Year == year);

        var caseNumber = $"{year}-{count + 1:D4}";

        var item = new Case
        {
            CompanyId = companyId,
            CaseNumber = caseNumber,
            Status = dto.Status,
            DeceasedFirstName = dto.DeceasedFirstName,
            DeceasedLastName = dto.DeceasedLastName,
            DateOfBirth = dto.DateOfBirth,
            DateOfDeath = dto.DateOfDeath,
            DestinationCountry = dto.DestinationCountry,
            DestinationCity = dto.DestinationCity,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Cases.Add(item);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { companyId, id = item.Id },
            item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int companyId,
        int id,
        int userId,
        UpdateCaseDto dto)
    {
        var item = await _context.Cases
            .FirstOrDefaultAsync(c =>
                c.Id == id &&
                c.CompanyId == companyId);

        if (item == null)
            return NotFound();

        if (dto.Status != null)
            item.Status = dto.Status;

        if (dto.DeceasedFirstName != null)
            item.DeceasedFirstName = dto.DeceasedFirstName;

        if (dto.DeceasedLastName != null)
            item.DeceasedLastName = dto.DeceasedLastName;

        if (dto.DateOfBirth.HasValue)
            item.DateOfBirth = dto.DateOfBirth;

        if (dto.DateOfDeath.HasValue)
            item.DateOfDeath = dto.DateOfDeath;

        if (dto.DestinationCountry != null)
            item.DestinationCountry = dto.DestinationCountry;

        if (dto.DestinationCity != null)
            item.DestinationCity = dto.DestinationCity;

        item.UpdatedBy = userId;
        item.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}