using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.DTOs.Contact;
using Funeral_Management_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management_Backend.Controllers;

[ApiController]
[Route("api/companies/{companyId:int}/contacts")]
public class ContactsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ContactsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int companyId)
    {
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

    [HttpPost]
    public async Task<IActionResult> Create(
        int companyId,
        CreateContactDto dto)
    {
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

        return Ok(contact);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int companyId,
        int id,
        UpdateContactDto dto)
    {
        var contact = await _context.Contacts
            .FirstOrDefaultAsync(c =>
                c.Id == id &&
                c.CompanyId == companyId);

        if (contact == null)
            return NotFound();

        contact.FirstName = dto.FirstName;
        contact.LastName = dto.LastName;
        contact.CompanyName = dto.CompanyName;
        contact.Phone = dto.Phone;
        contact.Email = dto.Email;
        contact.Language = dto.Language;
        contact.Notes = dto.Notes;
        contact.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}