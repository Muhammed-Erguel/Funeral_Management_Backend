using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.Dtos.Documents;
using Funeral_Management_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DocumentsController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/documents
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var documents = await _context.Documents
            .AsNoTracking()
            .Select(d => new
            {
                d.Id,
                d.CaseId,
                CaseNumber = d.Case.CaseNumber,

                d.DocumentTypeId,
                DocumentType = d.DocumentType.Name,

                d.UploadedBy,
                UploadedByEmail = d.UploadedByUser.Email,

                d.StorageKey,
                d.OriginalFilename,
                d.MimeType,
                d.SizeBytes,
                d.Status,
                d.Deadline,
                d.ReceivedAt,
                d.CreatedAt
            })
            .ToListAsync();

        return Ok(documents);
    }

    // GET /api/documents/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var document = await _context.Documents
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(d => new
            {
                d.Id,
                d.CaseId,
                CaseNumber = d.Case.CaseNumber,

                d.DocumentTypeId,
                DocumentType = d.DocumentType.Name,

                d.UploadedBy,
                UploadedByEmail = d.UploadedByUser.Email,

                d.StorageKey,
                d.OriginalFilename,
                d.MimeType,
                d.SizeBytes,
                d.Status,
                d.Deadline,
                d.ReceivedAt,
                d.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (document == null)
            return NotFound();

        return Ok(document);
    }

    // GET /api/documents/case/1
    [HttpGet("case/{caseId:int}")]
    public async Task<IActionResult> GetByCase(int caseId)
    {
        var documents = await _context.Documents
            .AsNoTracking()
            .Where(d => d.CaseId == caseId)
            .Select(d => new
            {
                d.Id,
                d.DocumentTypeId,
                DocumentType = d.DocumentType.Name,
                d.OriginalFilename,
                d.MimeType,
                d.SizeBytes,
                d.Status,
                d.Deadline,
                d.ReceivedAt,
                d.CreatedAt
            })
            .ToListAsync();

        return Ok(documents);
    }

    // POST /api/documents
    [HttpPost]
    public async Task<IActionResult> Create(CreateDocumentDto dto)
    {
        // Temporär für deine aktuellen API-Tests.
        // Später aus dem eingeloggten User auslesen.
        var userId = 1;

        var caseExists = await _context.Cases
            .AnyAsync(c => c.Id == dto.CaseId);

        if (!caseExists)
            return NotFound("Case not found.");

        var documentTypeExists = await _context.DocumentTypes
            .AnyAsync(dt => dt.Id == dto.DocumentTypeId);

        if (!documentTypeExists)
            return NotFound("Document type not found.");

        var document = new Document
        {
            CaseId = dto.CaseId,
            DocumentTypeId = dto.DocumentTypeId,

            UploadedBy = userId,

            StorageKey = dto.StorageKey,
            OriginalFilename = dto.OriginalFilename,
            MimeType = dto.MimeType,
            SizeBytes = dto.SizeBytes,

            Status = dto.Status,

            Deadline = dto.Deadline,
            ReceivedAt = dto.ReceivedAt,

            CreatedAt = DateTime.UtcNow
        };

        _context.Documents.Add(document);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = document.Id },
            new
            {
                document.Id,
                document.CaseId,
                document.DocumentTypeId,
                document.UploadedBy,
                document.StorageKey,
                document.OriginalFilename,
                document.MimeType,
                document.SizeBytes,
                document.Status,
                document.Deadline,
                document.ReceivedAt,
                document.CreatedAt
            });
    }

    // PUT /api/documents/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateDocumentDto dto)
    {
        var document = await _context.Documents
            .FirstOrDefaultAsync(d => d.Id == id);

        if (document == null)
            return NotFound();

        var documentTypeExists = await _context.DocumentTypes
            .AnyAsync(dt => dt.Id == dto.DocumentTypeId);

        if (!documentTypeExists)
            return NotFound("Document type not found.");

        document.DocumentTypeId = dto.DocumentTypeId;
        document.StorageKey = dto.StorageKey;
        document.OriginalFilename = dto.OriginalFilename;
        document.MimeType = dto.MimeType;
        document.SizeBytes = dto.SizeBytes;
        document.Status = dto.Status;
        document.Deadline = dto.Deadline;
        document.ReceivedAt = dto.ReceivedAt;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE /api/documents/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var document = await _context.Documents
            .FirstOrDefaultAsync(d => d.Id == id);

        if (document == null)
            return NotFound();

        _context.Documents.Remove(document);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}