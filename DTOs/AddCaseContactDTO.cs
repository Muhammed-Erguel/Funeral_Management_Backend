namespace Funeral_Management_Backend.DTOs.CaseContact;

public class AddCaseContactDto
{
    public int ContactId { get; set; }

    public string Role { get; set; } = string.Empty;
    public string? Notes { get; set; }
}