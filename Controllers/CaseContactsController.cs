using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.DTOs.CaseContact;
using Funeral_Management_Backend.Models;
using Funeral_Management_Backend.Services.Audit;
using Funeral_Management_Backend.Services.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management_Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/cases/{caseId:int}/contacts")]
public class CaseContactsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogService _auditLog;

    public CaseContactsController(
        AppDbContext context,
        ICurrentUserService currentUser,
        IAuditLogService auditLog)
    {
        _context = context;
        _currentUser = currentUser;
        _auditLog = auditLog;
    }

    // --------------------------------------------------
    // POST /api/cases/{caseId}/contacts
    // --------------------------------------------------

    [HttpPost]
    public async Task<IActionResult> Add(
        int caseId,
        AddCaseContactDto dto)
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
        // Contact prüfen
        // --------------------------------------------------

        var contact = await _context.Contacts
            .AsNoTracking()
            .FirstOrDefaultAsync(c =>
                c.Id == dto.ContactId &&
                c.CompanyId == companyId);

        if (contact == null)
        {
            return NotFound(new
            {
                message = "Contact not found."
            });
        }

        // --------------------------------------------------
        // Beziehung bereits vorhanden?
        // --------------------------------------------------

        var alreadyExists = await _context.CaseContacts
            .AnyAsync(cc =>
                cc.CaseId == caseId &&
                cc.ContactId == dto.ContactId &&
                cc.Role == dto.Role);

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

        var relation = new CaseContact
        {
            CaseId = caseId,
            ContactId = dto.ContactId,
            Role = dto.Role,
            Notes = dto.Notes
        };

        _context.CaseContacts.Add(relation);

        await _context.SaveChangesAsync();

        // --------------------------------------------------
        // Audit Log
        // --------------------------------------------------

        await _auditLog.LogAsync(
            action: "ContactAddedToCase",
            entityType: "CaseContact",
            entityId: dto.ContactId,
            caseId: caseId,
            metadata: new
            {
                ContactId = contact.Id,
                contact.FirstName,
                contact.LastName,
                Role = dto.Role,
                Notes = dto.Notes
            }
        );

        return Ok(relation);
    }

    // --------------------------------------------------
    // DELETE /api/cases/{caseId}/contacts/{contactId}
    // ?role=...
    // --------------------------------------------------

    [HttpDelete("{contactId:int}")]
    public async Task<IActionResult> Delete(
        int caseId,
        int contactId,
        [FromQuery] string role)
    {
        var companyId = _currentUser.CompanyId;

        // --------------------------------------------------
        // Beziehung suchen
        // --------------------------------------------------

        var relation = await _context.CaseContacts
            .Include(cc => cc.Contact)
            .FirstOrDefaultAsync(cc =>
                cc.CaseId == caseId &&
                cc.ContactId == contactId &&
                cc.Role == role &&
                cc.Case.CompanyId == companyId);

        if (relation == null)
            return NotFound();

        // --------------------------------------------------
        // Daten vor dem Löschen sichern
        // --------------------------------------------------

        var deletedRelation = new
        {
            relation.ContactId,
            relation.Contact.FirstName,
            relation.Contact.LastName,
            relation.Role,
            relation.Notes
        };

        // --------------------------------------------------
        // Beziehung löschen
        // --------------------------------------------------

        _context.CaseContacts.Remove(relation);

        await _context.SaveChangesAsync();

        // --------------------------------------------------
        // Audit Log
        // --------------------------------------------------

        await _auditLog.LogAsync(
            action: "ContactRemovedFromCase",
            entityType: "CaseContact",
            entityId: contactId,
            caseId: caseId,
            metadata: deletedRelation
        );

        return NoContent();
    }
}