namespace Funeral_Management_Backend.DTOs.DocumentType;

public class UpdateDocumentTypeDto
{
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
    public int? RetentionMonths { get; set; }
}