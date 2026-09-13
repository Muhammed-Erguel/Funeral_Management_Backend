namespace Funeral_Management.DTOs.Document;

public class DocumentResponseDto
{
    public int Id { get; set; }
    public int CaseId { get; set; }
    public int DocumentTypeId { get; set; }

    public string OriginalFilename { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime? Deadline { get; set; }
    public DateTime? ReceivedAt { get; set; }

    public DateTime CreatedAt { get; set; }
}