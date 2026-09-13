namespace Funeral_Management_Backend.DTOs.DocumentType;

public class CreateDocumentTypeDto
{
    public string Name { get; set; } = string.Empty;
    public int? RetentionMonths { get; set; }
}