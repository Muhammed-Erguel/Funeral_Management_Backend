namespace Funeral_Management_Backend.Dtos.Documents;

public class CreateDocumentDto
{
    public int CaseId { get; set; }
    public int DocumentTypeId { get; set; }

    public string StorageKey { get; set; } = string.Empty;
    public string OriginalFilename { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;

    public long SizeBytes { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime? Deadline { get; set; }
    public DateTime? ReceivedAt { get; set; }
}