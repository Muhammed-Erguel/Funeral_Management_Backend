namespace Funeral_Management_Backend.Models;

public class Document
{
    public int Id { get; set; }

    public int CaseId { get; set; }
    public Case Case { get; set; } = null!;

    public int DocumentTypeId { get; set; }
    public DocumentType DocumentType { get; set; } = null!;

    public int UploadedBy { get; set; }
    public User UploadedByUser { get; set; } = null!;

    public string StorageKey { get; set; } = string.Empty;
    public string OriginalFilename { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;

    public long SizeBytes { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime? Deadline { get; set; }
    public DateTime? ReceivedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}