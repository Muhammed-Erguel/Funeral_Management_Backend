using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.DTOs.Contact;
using Funeral_Management_Backend.Models;
using Funeral_Management_Backend.Services.CurrentUser;
using Funeral_Management_Backend.Services.Audit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management_Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/contacts")]
public class ContactsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogService _auditLog;

    public ContactsController(
        AppDbContext context,
        ICurrentUserService currentUser,
        IAuditLogService auditLog)
    {
        _context = context;
        _currentUser = currentUser;
        _auditLog = auditLog;
    }

    // --------------------------------------------------
    // GET /api/contacts
    // --------------------------------------------------

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var companyId = _currentUser.CompanyId;

        var contacts = await _context.Contacts
            .AsNoTracking()
            .Where(c => c.CompanyId == companyId)
            .Select(c => new ContactResponseDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                CompanyName = c.CompanyName,
                Phone = c.Phone,
                Email = c.Email,
                Language = c.Language,
                Notes = c.Notes
            })
            .ToListAsync();

        return Ok(contacts);
    }

    // --------------------------------------------------
    // GET /api/contacts/5
    // --------------------------------------------------

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var companyId = _currentUser.CompanyId;

        var contact = await _context.Contacts
            .AsNoTracking()
            .Where(c =>
                c.Id == id &&
                c.CompanyId == companyId)
            .Select(c => new ContactResponseDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                CompanyName = c.CompanyName,
                Phone = c.Phone,
                Email = c.Email,
                Language = c.Language,
                Notes = c.Notes
            })
            .FirstOrDefaultAsync();

        if (contact == null)
            return NotFound();

        return Ok(contact);
    }

    // --------------------------------------------------
    // POST /api/contacts
    // --------------------------------------------------

    [HttpPost]
    public async Task<IActionResult> Create(CreateContactDto dto)
    {
        var companyId = _currentUser.CompanyId;

        var contact = new Contact
        {
            CompanyId = companyId,

            FirstName = dto.FirstName,
            LastName = dto.LastName,
            CompanyName = dto.CompanyName,

            Phone = dto.Phone,
            Email = dto.Email,
            Language = dto.Language,
            Notes = dto.Notes,

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Contacts.Add(contact);

        await _context.SaveChangesAsync();

        await _auditLog.LogAsync(
            action: "ContactCreated",
            entityType: "Contact",
            entityId: contact.Id,
            caseId: null,
            metadata: new
            {
                contact.FirstName,
                contact.LastName
            }
        );

        var response = new ContactResponseDto
        {
            Id = contact.Id,
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            CompanyName = contact.CompanyName,
            Phone = contact.Phone,
            Email = contact.Email,
            Language = contact.Language,
            Notes = contact.Notes
        };

        return CreatedAtAction(
            nameof(GetById),
            new { id = contact.Id },
            response
        );
    }

    // --------------------------------------------------
    // PUT /api/contacts/5
    // --------------------------------------------------

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
    int id,
    UpdateContactDto dto)
    {
        var companyId = _currentUser.CompanyId;

        var contact = await _context.Contacts
            .FirstOrDefaultAsync(c =>
                c.Id == id &&
                c.CompanyId == companyId);

        if (contact == null)
            return NotFound();

        // --------------------------------------------------
        // Alte Werte für Audit Log speichern
        // --------------------------------------------------

        var oldFirstName = contact.FirstName;
        var oldLastName = contact.LastName;
        var oldCompanyName = contact.CompanyName;
        var oldPhone = contact.Phone;
        var oldEmail = contact.Email;
        var oldLanguage = contact.Language;
        var oldNotes = contact.Notes;

        // --------------------------------------------------
        // Contact aktualisieren
        // --------------------------------------------------

        if (dto.FirstName != null)
            contact.FirstName = dto.FirstName;

        if (dto.LastName != null)
            contact.LastName = dto.LastName;

        if (dto.CompanyName != null)
            contact.CompanyName = dto.CompanyName;

        if (dto.Phone != null)
            contact.Phone = dto.Phone;

        if (dto.Email != null)
            contact.Email = dto.Email;

        if (dto.Language != null)
            contact.Language = dto.Language;

        if (dto.Notes != null)
            contact.Notes = dto.Notes;

        contact.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // --------------------------------------------------
        // Audit Log
        // --------------------------------------------------

        await _auditLog.LogAsync(
            action: "ContactUpdated",
            entityType: "Contact",
            entityId: contact.Id,
            caseId: null,
            metadata: new
            {
                Old = new
                {
                    FirstName = oldFirstName,
                    LastName = oldLastName,
                    CompanyName = oldCompanyName,
                    Phone = oldPhone,
                    Email = oldEmail,
                    Language = oldLanguage,
                    Notes = oldNotes
                },

                New = new
                {
                    contact.FirstName,
                    contact.LastName,
                    contact.CompanyName,
                    contact.Phone,
                    contact.Email,
                    contact.Language,
                    contact.Notes
                }
            }
        );

        return NoContent();
    }

    // --------------------------------------------------
    // DELETE /api/contacts/5
    // --------------------------------------------------

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = _currentUser.CompanyId;

        var contact = await _context.Contacts
            .FirstOrDefaultAsync(c =>
                c.Id == id &&
                c.CompanyId == companyId);

        if (contact == null)
            return NotFound();

        // --------------------------------------------------
        // Daten für Audit Log vor dem Löschen sichern
        // --------------------------------------------------

        var deletedContact = new
        {
            contact.Id,
            contact.FirstName,
            contact.LastName,
            contact.CompanyName,
            contact.Phone,
            contact.Email,
            contact.Language
        };

        // --------------------------------------------------
        // Contact löschen
        // --------------------------------------------------

        _context.Contacts.Remove(contact);

        await _context.SaveChangesAsync();

        // --------------------------------------------------
        // Audit Log
        // --------------------------------------------------

        await _auditLog.LogAsync(
            action: "ContactDeleted",
            entityType: "Contact",
            entityId: id,
            caseId: null,
            metadata: deletedContact
        );

        return NoContent();
    }
}