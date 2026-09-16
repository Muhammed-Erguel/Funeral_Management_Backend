using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.DTOs.DocumentType;
using Funeral_Management_Backend.Models;
using Funeral_Management_Backend.Services.Audit;
using Funeral_Management_Backend.Services.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management_Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/document-types")]
public class DocumentTypesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogService _auditLog;

    public DocumentTypesController(
        AppDbContext context,
        ICurrentUserService currentUser,
        IAuditLogService auditLog)
    {
        _context = context;
        _currentUser = currentUser;
        _auditLog = auditLog;
    }

    // --------------------------------------------------
    // GET /api/document-types
    // --------------------------------------------------

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var companyId = _currentUser.CompanyId;

        var items = await _context.DocumentTypes
            .AsNoTracking()
            .Where(d => d.CompanyId == companyId)
            .ToListAsync();

        return Ok(items);
    }

    // --------------------------------------------------
    // POST /api/document-types
    // --------------------------------------------------

    [HttpPost]
    [Authorize(Roles = "Owner,Admin")]
    public async Task<IActionResult> Create(
        CreateDocumentTypeDto dto)
    {
        var companyId = _currentUser.CompanyId;

        var item = new DocumentType
        {
            CompanyId = companyId,
            Name = dto.Name,
            IsActive = true,
            RetentionMonths = dto.RetentionMonths,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.DocumentTypes.Add(item);

        await _context.SaveChangesAsync();

        // --------------------------------------------------
        // Audit Log
        // --------------------------------------------------

        await _auditLog.LogAsync(
            action: "DocumentTypeCreated",
            entityType: "DocumentType",
            entityId: item.Id,
            caseId: null,
            metadata: new
            {
                item.Name,
                item.IsActive,
                item.RetentionMonths
            }
        );

        return Ok(item);
    }

    // --------------------------------------------------
    // PUT /api/document-types/{id}
    // --------------------------------------------------

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Owner,Admin")]
    public async Task<IActionResult> Update(
        int id,
        UpdateDocumentTypeDto dto)
    {
        var companyId = _currentUser.CompanyId;

        var item = await _context.DocumentTypes
            .FirstOrDefaultAsync(d =>
                d.Id == id &&
                d.CompanyId == companyId);

        if (item == null)
            return NotFound();

        // --------------------------------------------------
        // Alte Werte sichern
        // --------------------------------------------------

        var oldName = item.Name;
        var oldIsActive = item.IsActive;
        var oldRetentionMonths = item.RetentionMonths;

        // --------------------------------------------------
        // Update
        // --------------------------------------------------

        if (dto.Name != null)
            item.Name = dto.Name;

        if (dto.IsActive.HasValue)
            item.IsActive = dto.IsActive.Value;

        if (dto.RetentionMonths.HasValue)
            item.RetentionMonths = dto.RetentionMonths;

        item.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // --------------------------------------------------
        // Audit Log
        // --------------------------------------------------

        await _auditLog.LogAsync(
            action: "DocumentTypeUpdated",
            entityType: "DocumentType",
            entityId: item.Id,
            caseId: null,
            metadata: new
            {
                Old = new
                {
                    Name = oldName,
                    IsActive = oldIsActive,
                    RetentionMonths = oldRetentionMonths
                },

                New = new
                {
                    item.Name,
                    item.IsActive,
                    item.RetentionMonths
                }
            }
        );

        return NoContent();
    }

    // --------------------------------------------------
    // DELETE /api/document-types/{id}
    //
    // Kein echtes Löschen.
    // DocumentType wird deaktiviert.
    // --------------------------------------------------

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Owner,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = _currentUser.CompanyId;

        var item = await _context.DocumentTypes
            .FirstOrDefaultAsync(d =>
                d.Id == id &&
                d.CompanyId == companyId);

        if (item == null)
            return NotFound();

        // Bereits deaktiviert
        if (!item.IsActive)
        {
            return BadRequest(new
            {
                message = "Document type is already inactive."
            });
        }

        item.IsActive = false;
        item.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // --------------------------------------------------
        // Audit Log
        // --------------------------------------------------

        await _auditLog.LogAsync(
            action: "DocumentTypeDeactivated",
            entityType: "DocumentType",
            entityId: item.Id,
            caseId: null,
            metadata: new
            {
                item.Name,
                item.RetentionMonths
            }
        );

        return NoContent();
    }
}