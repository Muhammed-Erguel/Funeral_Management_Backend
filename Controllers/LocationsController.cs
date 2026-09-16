using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.DTOs.Location;
using Funeral_Management_Backend.Models;
using Funeral_Management_Backend.Services.Audit;
using Funeral_Management_Backend.Services.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management_Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogService _auditLog;

    public LocationsController(
        AppDbContext context,
        ICurrentUserService currentUser,
        IAuditLogService auditLog)
    {
        _context = context;
        _currentUser = currentUser;
        _auditLog = auditLog;
    }

    // --------------------------------------------------
    // GET /api/locations
    // --------------------------------------------------

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var companyId = _currentUser.CompanyId;

        var locations = await _context.Locations
            .AsNoTracking()
            .Where(l => l.CompanyId == companyId)
            .ToListAsync();

        return Ok(locations);
    }

    // --------------------------------------------------
    // GET /api/locations/{id}
    // --------------------------------------------------

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var companyId = _currentUser.CompanyId;

        var location = await _context.Locations
            .AsNoTracking()
            .FirstOrDefaultAsync(l =>
                l.Id == id &&
                l.CompanyId == companyId);

        if (location == null)
            return NotFound();

        return Ok(location);
    }

    // --------------------------------------------------
    // POST /api/locations
    // --------------------------------------------------

    [HttpPost]
    public async Task<IActionResult> Create(CreateLocationDto dto)
    {
        var companyId = _currentUser.CompanyId;

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

        // --------------------------------------------------
        // Audit Log
        // --------------------------------------------------

        await _auditLog.LogAsync(
            action: "LocationCreated",
            entityType: "Location",
            entityId: location.Id,
            caseId: null,
            metadata: new
            {
                location.Name,
                location.Type,
                location.City,
                location.Country
            }
        );

        return CreatedAtAction(
            nameof(GetById),
            new { id = location.Id },
            location
        );
    }

    // --------------------------------------------------
    // PUT /api/locations/{id}
    // --------------------------------------------------

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateLocationDto dto)
    {
        var companyId = _currentUser.CompanyId;

        var location = await _context.Locations
            .FirstOrDefaultAsync(l =>
                l.Id == id &&
                l.CompanyId == companyId);

        if (location == null)
            return NotFound();

        // --------------------------------------------------
        // Alte Werte für Audit Log sichern
        // --------------------------------------------------

        var oldName = location.Name;
        var oldType = location.Type;
        var oldStreet = location.Street;
        var oldPostalCode = location.PostalCode;
        var oldCity = location.City;
        var oldCountry = location.Country;
        var oldPhone = location.Phone;
        var oldEmail = location.Email;
        var oldOpeningHours = location.OpeningHours;
        var oldNotes = location.Notes;

        // --------------------------------------------------
        // Location aktualisieren
        // --------------------------------------------------

        if (dto.Name != null)
            location.Name = dto.Name;

        if (dto.Type != null)
            location.Type = dto.Type;

        if (dto.Street != null)
            location.Street = dto.Street;

        if (dto.PostalCode != null)
            location.PostalCode = dto.PostalCode;

        if (dto.City != null)
            location.City = dto.City;

        if (dto.Country != null)
            location.Country = dto.Country;

        if (dto.Phone != null)
            location.Phone = dto.Phone;

        if (dto.Email != null)
            location.Email = dto.Email;

        if (dto.OpeningHours != null)
            location.OpeningHours = dto.OpeningHours;

        if (dto.Notes != null)
            location.Notes = dto.Notes;

        location.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // --------------------------------------------------
        // Audit Log
        // --------------------------------------------------

        await _auditLog.LogAsync(
            action: "LocationUpdated",
            entityType: "Location",
            entityId: location.Id,
            caseId: null,
            metadata: new
            {
                Old = new
                {
                    Name = oldName,
                    Type = oldType,
                    Street = oldStreet,
                    PostalCode = oldPostalCode,
                    City = oldCity,
                    Country = oldCountry,
                    Phone = oldPhone,
                    Email = oldEmail,
                    OpeningHours = oldOpeningHours,
                    Notes = oldNotes
                },

                New = new
                {
                    location.Name,
                    location.Type,
                    location.Street,
                    location.PostalCode,
                    location.City,
                    location.Country,
                    location.Phone,
                    location.Email,
                    location.OpeningHours,
                    location.Notes
                }
            }
        );

        return NoContent();
    }

    // --------------------------------------------------
    // DELETE /api/locations/{id}
    // --------------------------------------------------

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = _currentUser.CompanyId;

        var location = await _context.Locations
            .FirstOrDefaultAsync(l =>
                l.Id == id &&
                l.CompanyId == companyId);

        if (location == null)
            return NotFound();

        // --------------------------------------------------
        // Daten vor dem Löschen sichern
        // --------------------------------------------------

        var deletedLocation = new
        {
            location.Id,
            location.Name,
            location.Type,
            location.Street,
            location.PostalCode,
            location.City,
            location.Country,
            location.Phone,
            location.Email,
            location.OpeningHours,
            location.Notes
        };

        // --------------------------------------------------
        // Location löschen
        // --------------------------------------------------

        _context.Locations.Remove(location);

        await _context.SaveChangesAsync();

        // --------------------------------------------------
        // Audit Log
        // --------------------------------------------------

        await _auditLog.LogAsync(
            action: "LocationDeleted",
            entityType: "Location",
            entityId: id,
            caseId: null,
            metadata: deletedLocation
        );

        return NoContent();
    }
}