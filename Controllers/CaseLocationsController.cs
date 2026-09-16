using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.DTOs.CaseLocation;
using Funeral_Management_Backend.Models;
using Funeral_Management_Backend.Services.Audit;
using Funeral_Management_Backend.Services.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management_Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/cases/{caseId:int}/locations")]
public class CaseLocationsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogService _auditLog;

    public CaseLocationsController(
        AppDbContext context,
        ICurrentUserService currentUser,
        IAuditLogService auditLog)
    {
        _context = context;
        _currentUser = currentUser;
        _auditLog = auditLog;
    }

    // --------------------------------------------------
    // POST /api/cases/{caseId}/locations
    // --------------------------------------------------

    [HttpPost]
    public async Task<IActionResult> Add(
        int caseId,
        AddCaseLocationDto dto)
    {
        var companyId = _currentUser.CompanyId;

        // --------------------------------------------------
        // Case prüfen
        // --------------------------------------------------

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

        // --------------------------------------------------
        // Location prüfen
        // --------------------------------------------------

        var location = await _context.Locations
            .AsNoTracking()
            .FirstOrDefaultAsync(l =>
                l.Id == dto.LocationId &&
                l.CompanyId == companyId);

        if (location == null)
        {
            return NotFound(new
            {
                message = "Location not found."
            });
        }

        // --------------------------------------------------
        // Beziehung bereits vorhanden?
        // --------------------------------------------------

        var alreadyExists = await _context.CaseLocations
            .AnyAsync(cl =>
                cl.CaseId == caseId &&
                cl.LocationId == dto.LocationId &&
                cl.Role == dto.Role);

        if (alreadyExists)
        {
            return Conflict(new
            {
                message = "Relation already exists."
            });
        }

        // --------------------------------------------------
        // Beziehung erstellen
        // --------------------------------------------------

        var relation = new CaseLocation
        {
            CaseId = caseId,
            LocationId = dto.LocationId,
            Role = dto.Role,
            Notes = dto.Notes
        };

        _context.CaseLocations.Add(relation);

        await _context.SaveChangesAsync();

        // --------------------------------------------------
        // Audit Log
        // --------------------------------------------------

        await _auditLog.LogAsync(
            action: "LocationAddedToCase",
            entityType: "CaseLocation",
            entityId: dto.LocationId,
            caseId: caseId,
            metadata: new
            {
                LocationId = location.Id,
                location.Name,
                location.Type,
                location.City,
                Role = dto.Role,
                Notes = dto.Notes
            }
        );

        return Ok(relation);
    }

    // --------------------------------------------------
    // DELETE /api/cases/{caseId}/locations/{locationId}
    // ?role=...
    // --------------------------------------------------

    [HttpDelete("{locationId:int}")]
    public async Task<IActionResult> Delete(
        int caseId,
        int locationId,
        [FromQuery] string role)
    {
        var companyId = _currentUser.CompanyId;

        // --------------------------------------------------
        // Beziehung suchen
        // --------------------------------------------------

        var relation = await _context.CaseLocations
            .Include(cl => cl.Location)
            .FirstOrDefaultAsync(cl =>
                cl.CaseId == caseId &&
                cl.LocationId == locationId &&
                cl.Role == role &&
                cl.Case.CompanyId == companyId);

        if (relation == null)
            return NotFound();

        // --------------------------------------------------
        // Daten vor dem Löschen sichern
        // --------------------------------------------------

        var deletedRelation = new
        {
            relation.LocationId,
            relation.Location.Name,
            relation.Location.Type,
            relation.Location.City,
            relation.Role,
            relation.Notes
        };

        // --------------------------------------------------
        // Beziehung löschen
        // --------------------------------------------------

        _context.CaseLocations.Remove(relation);

        await _context.SaveChangesAsync();

        // --------------------------------------------------
        // Audit Log
        // --------------------------------------------------

        await _auditLog.LogAsync(
            action: "LocationRemovedFromCase",
            entityType: "CaseLocation",
            entityId: locationId,
            caseId: caseId,
            metadata: deletedRelation
        );

        return NoContent();
    }
}