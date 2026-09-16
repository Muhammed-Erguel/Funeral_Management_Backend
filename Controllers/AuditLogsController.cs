using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.Services.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management_Backend.Controllers;

[Authorize(Roles = "Owner,Admin")]
[ApiController]
[Route("api/audit-logs")]
public class AuditLogsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AuditLogsController(
        AppDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    // --------------------------------------------------
    // GET /api/audit-logs
    // --------------------------------------------------

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var companyId = _currentUser.CompanyId;

        var logs = await _context.AuditLogs
            .AsNoTracking()
            .Where(a => a.CompanyId == companyId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new
            {
                a.Id,
                a.CompanyId,
                a.UserId,
                a.CaseId,
                a.Action,
                a.EntityType,
                a.EntityId,
                a.Metadata,
                a.CreatedAt
            })
            .ToListAsync();

        return Ok(logs);
    }

    // --------------------------------------------------
    // GET /api/audit-logs/{id}
    // --------------------------------------------------

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var companyId = _currentUser.CompanyId;

        var log = await _context.AuditLogs
            .AsNoTracking()
            .Where(a =>
                a.Id == id &&
                a.CompanyId == companyId)
            .Select(a => new
            {
                a.Id,
                a.CompanyId,
                a.UserId,
                a.CaseId,
                a.Action,
                a.EntityType,
                a.EntityId,
                a.Metadata,
                a.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (log == null)
            return NotFound();

        return Ok(log);
    }

    // --------------------------------------------------
    // GET /api/audit-logs/case/{caseId}
    // --------------------------------------------------

    [HttpGet("case/{caseId:int}")]
    public async Task<IActionResult> GetByCase(int caseId)
    {
        var companyId = _currentUser.CompanyId;

        // Sicherstellen, dass der Case zur eigenen Firma gehört
        var caseExists = await _context.Cases
            .AnyAsync(c =>
                c.Id == caseId &&
                c.CompanyId == companyId);

        if (!caseExists)
        {
            return NotFound(new
            {
                message = "Case not found."
            });
        }

        var logs = await _context.AuditLogs
            .AsNoTracking()
            .Where(a =>
                a.CompanyId == companyId &&
                a.CaseId == caseId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new
            {
                a.Id,
                a.UserId,
                a.Action,
                a.EntityType,
                a.EntityId,
                a.Metadata,
                a.CreatedAt
            })
            .ToListAsync();

        return Ok(logs);
    }
}