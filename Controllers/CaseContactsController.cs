using Funeral_Management_Backend.Data;
using Funeral_Management.DTOs.CaseContact;
using Funeral_Management_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management.Controllers;

[ApiController]
[Route("api/cases/{caseId:int}/contacts")]
public class CaseContactsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CaseContactsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        int caseId,
        AddCaseContactDto dto)
    {
        var caseExists = await _context.Cases
            .AnyAsync(c => c.Id == caseId);

        if (!caseExists)
            return NotFound("Case not found.");

        var contactExists = await _context.Contacts
            .AnyAsync(c => c.Id == dto.ContactId);

        if (!contactExists)
            return NotFound("Contact not found.");

        var relation = new CaseContact
        {
            CaseId = caseId,
            ContactId = dto.ContactId,
            Role = dto.Role,
            Notes = dto.Notes
        };

        _context.CaseContacts.Add(relation);
        await _context.SaveChangesAsync();

        return Ok(relation);
    }

    [HttpDelete("{contactId:int}")]
    public async Task<IActionResult> Delete(
        int caseId,
        int contactId,
        string role)
    {
        var relation = await _context.CaseContacts
            .FirstOrDefaultAsync(cc =>
                cc.CaseId == caseId &&
                cc.ContactId == contactId &&
                cc.Role == role);

        if (relation == null)
            return NotFound();

        _context.CaseContacts.Remove(relation);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}