using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.DTOs.DocumentType;
using Funeral_Management_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management_Backend.Controllers;

[ApiController]
[Route("api/companies/{companyId:int}/document-types")]
public class DocumentTypesController : ControllerBase
{
    private readonly AppDbContext _context;

    public DocumentTypesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int companyId)
    {
        var items = await _context.DocumentTypes
            .AsNoTracking()
            .Where(d => d.CompanyId == companyId)
            .ToListAsync();

        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        int companyId,
        CreateDocumentTypeDto dto)
    {
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

        return Ok(item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int companyId,
        int id,
        UpdateDocumentTypeDto dto)
    {
        var item = await _context.DocumentTypes
            .FirstOrDefaultAsync(d =>
                d.Id == id &&
                d.CompanyId == companyId);

        if (item == null)
            return NotFound();

        if (dto.Name != null)
            item.Name = dto.Name;

        if (dto.IsActive.HasValue)
            item.IsActive = dto.IsActive.Value;

        if (dto.RetentionMonths.HasValue)
            item.RetentionMonths = dto.RetentionMonths;

        item.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}