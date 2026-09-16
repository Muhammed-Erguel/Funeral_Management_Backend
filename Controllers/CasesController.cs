using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.DTOs.Case;
using Funeral_Management_Backend.Models;
using Funeral_Management_Backend.Services.CurrentUser;
using Funeral_Management_Backend.Services.Audit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management_Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/cases")]
public class CasesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogService _auditLog;

    public CasesController(
        AppDbContext context,
        ICurrentUserService currentUser,
        IAuditLogService auditLog)
    {
        _context = context;
        _currentUser = currentUser;
        _auditLog = auditLog;
    }

    // --------------------------------------------------
    // GET /api/cases
    // --------------------------------------------------

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var companyId = _currentUser.CompanyId;

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

    // --------------------------------------------------
    // GET /api/cases/5
    // --------------------------------------------------

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var companyId = _currentUser.CompanyId;

        var item = await _context.Cases
            .AsNoTracking()
            .Where(c =>
                c.Id == id &&
                c.CompanyId == companyId)
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

    // --------------------------------------------------
    // POST /api/cases
    // --------------------------------------------------

    [HttpPost]
    public async Task<IActionResult> Create(CreateCaseDto dto)
    {
        var companyId = _currentUser.CompanyId;
        var userId = _currentUser.UserId;

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

        await _auditLog.LogAsync(
            action: "CaseCreated",
            entityType: "Case",
            entityId: item.Id,
            caseId: item.Id,
            metadata: new
            {
                item.CaseNumber,
                item.Status
            }
        );

        var response = new CaseResponseDto
        {
            Id = item.Id,
            CaseNumber = item.CaseNumber,
            Status = item.Status,
            DeceasedFirstName = item.DeceasedFirstName,
            DeceasedLastName = item.DeceasedLastName,
            DateOfBirth = item.DateOfBirth,
            DateOfDeath = item.DateOfDeath,
            DestinationCountry = item.DestinationCountry,
            DestinationCity = item.DestinationCity,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };

        return CreatedAtAction(
            nameof(GetById),
            new { id = item.Id },
            response
        );
    }

    // --------------------------------------------------
    // PUT /api/cases/5
    // --------------------------------------------------

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateCaseDto dto)
    {
        var companyId = _currentUser.CompanyId;
        var userId = _currentUser.UserId;

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

        await _auditLog.LogAsync(
            action: "CaseUpdated",
            entityType: "Case",
            entityId: item.Id,
            caseId: item.Id,
            metadata: new
            {
                item.CaseNumber,
                item.Status
            }
        );

        return NoContent();
    }
}