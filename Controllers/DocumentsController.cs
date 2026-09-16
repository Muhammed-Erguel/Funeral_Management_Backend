using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.Dtos.Documents;
using Funeral_Management_Backend.Models;
using Funeral_Management_Backend.Services.Audit;
using Funeral_Management_Backend.Services.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management_Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/documents")]
public class DocumentsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogService _auditLog;

    public DocumentsController(
        AppDbContext context,
        ICurrentUserService currentUser,
        IAuditLogService auditLog)
    {
        _context = context;
        _currentUser = currentUser;
        _auditLog = auditLog;
    }

    // --------------------------------------------------
    // GET /api/documents
    // --------------------------------------------------

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var companyId = _currentUser.CompanyId;

        var documents = await _context.Documents
            .AsNoTracking()

            // Document gehört über den Case zur Company
            .Where(d => d.Case.CompanyId == companyId)

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

    // --------------------------------------------------
    // GET /api/documents/{id}
    // --------------------------------------------------

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var companyId = _currentUser.CompanyId;

        var document = await _context.Documents
            .AsNoTracking()
            .Where(d =>
                d.Id == id &&
                d.Case.CompanyId == companyId)
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

    // --------------------------------------------------
    // GET /api/documents/case/{caseId}
    // --------------------------------------------------

    [HttpGet("case/{caseId:int}")]
    public async Task<IActionResult> GetByCase(int caseId)
    {
        var companyId = _currentUser.CompanyId;

        // Sicherstellen, dass der Case wirklich
        // zur eingeloggten Company gehört.
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

    // --------------------------------------------------
    // POST /api/documents
    // --------------------------------------------------

    [HttpPost]
    public async Task<IActionResult> Create(CreateDocumentDto dto)
    {
        var companyId = _currentUser.CompanyId;
        var userId = _currentUser.UserId;

        // --------------------------------------------------
        // Case prüfen
        // --------------------------------------------------

        var caseExists = await _context.Cases
            .AnyAsync(c =>
                c.Id == dto.CaseId &&
                c.CompanyId == companyId);

        if (!caseExists)
        {
            return NotFound(new
            {
                message = "Case not found."
            });
        }

        // --------------------------------------------------
        // DocumentType prüfen
        // --------------------------------------------------

        var documentTypeExists = await _context.DocumentTypes
            .AnyAsync(dt =>
                dt.Id == dto.DocumentTypeId &&
                dt.CompanyId == companyId);

        if (!documentTypeExists)
        {
            return NotFound(new
            {
                message = "Document type not found."
            });
        }

        // --------------------------------------------------
        // Document erstellen
        // --------------------------------------------------

        var document = new Document
        {
            CaseId = dto.CaseId,
            DocumentTypeId = dto.DocumentTypeId,

            // Nicht mehr hardcoded!
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

        // --------------------------------------------------
        // Audit Log
        // --------------------------------------------------

        await _auditLog.LogAsync(
            action: "DocumentCreated",
            entityType: "Document",
            entityId: document.Id,
            caseId: document.CaseId,
            metadata: new
            {
                document.DocumentTypeId,
                document.OriginalFilename,
                document.MimeType,
                document.SizeBytes,
                document.Status
            }
        );

        // --------------------------------------------------
        // Response
        // --------------------------------------------------

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

    // --------------------------------------------------
    // PUT /api/documents/{id}
    // --------------------------------------------------

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateDocumentDto dto)
    {
        var companyId = _currentUser.CompanyId;

        var document = await _context.Documents
            .FirstOrDefaultAsync(d =>
                d.Id == id &&
                d.Case.CompanyId == companyId);

        if (document == null)
            return NotFound();

        // --------------------------------------------------
        // DocumentType prüfen
        // --------------------------------------------------

        var documentTypeExists = await _context.DocumentTypes
            .AnyAsync(dt =>
                dt.Id == dto.DocumentTypeId &&
                dt.CompanyId == companyId);

        if (!documentTypeExists)
        {
            return NotFound(new
            {
                message = "Document type not found."
            });
        }

        // --------------------------------------------------
        // Alte Werte sichern
        // --------------------------------------------------

        var oldDocumentTypeId = document.DocumentTypeId;
        var oldStorageKey = document.StorageKey;
        var oldOriginalFilename = document.OriginalFilename;
        var oldMimeType = document.MimeType;
        var oldSizeBytes = document.SizeBytes;
        var oldStatus = document.Status;
        var oldDeadline = document.Deadline;
        var oldReceivedAt = document.ReceivedAt;

        // --------------------------------------------------
        // Update
        // --------------------------------------------------

        document.DocumentTypeId = dto.DocumentTypeId;
        document.StorageKey = dto.StorageKey;
        document.OriginalFilename = dto.OriginalFilename;
        document.MimeType = dto.MimeType;
        document.SizeBytes = dto.SizeBytes;
        document.Status = dto.Status;
        document.Deadline = dto.Deadline;
        document.ReceivedAt = dto.ReceivedAt;

        await _context.SaveChangesAsync();

        // --------------------------------------------------
        // Audit Log
        // --------------------------------------------------

        await _auditLog.LogAsync(
            action: "DocumentUpdated",
            entityType: "Document",
            entityId: document.Id,
            caseId: document.CaseId,
            metadata: new
            {
                Old = new
                {
                    DocumentTypeId = oldDocumentTypeId,
                    StorageKey = oldStorageKey,
                    OriginalFilename = oldOriginalFilename,
                    MimeType = oldMimeType,
                    SizeBytes = oldSizeBytes,
                    Status = oldStatus,
                    Deadline = oldDeadline,
                    ReceivedAt = oldReceivedAt
                },

                New = new
                {
                    document.DocumentTypeId,
                    document.StorageKey,
                    document.OriginalFilename,
                    document.MimeType,
                    document.SizeBytes,
                    document.Status,
                    document.Deadline,
                    document.ReceivedAt
                }
            }
        );

        return NoContent();
    }

    // --------------------------------------------------
    // DELETE /api/documents/{id}
    // --------------------------------------------------

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = _currentUser.CompanyId;

        var document = await _context.Documents
            .FirstOrDefaultAsync(d =>
                d.Id == id &&
                d.Case.CompanyId == companyId);

        if (document == null)
            return NotFound();

        // --------------------------------------------------
        // Daten vor dem Löschen sichern
        // --------------------------------------------------

        var caseId = document.CaseId;

        var deletedDocument = new
        {
            document.Id,
            document.DocumentTypeId,
            document.OriginalFilename,
            document.MimeType,
            document.SizeBytes,
            document.Status,
            document.Deadline,
            document.ReceivedAt
        };

        // --------------------------------------------------
        // Document löschen
        // --------------------------------------------------

        _context.Documents.Remove(document);

        await _context.SaveChangesAsync();

        // --------------------------------------------------
        // Audit Log
        // --------------------------------------------------

        await _auditLog.LogAsync(
            action: "DocumentDeleted",
            entityType: "Document",
            entityId: id,
            caseId: caseId,
            metadata: deletedDocument
        );

        return NoContent();
    }
}