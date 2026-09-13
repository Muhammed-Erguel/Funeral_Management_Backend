namespace Funeral_Management.DTOs.Document;

public class CreateDocumentDto
{
    public int DocumentTypeId { get; set; }

    public string Status { get; set; } = string.Empty;
    public DateTime? Deadline { get; set; }
    public DateTime? ReceivedAt { get; set; }
}