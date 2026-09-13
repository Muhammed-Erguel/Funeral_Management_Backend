namespace Funeral_Management_Backend.Models;

public class CaseContact
{
    public int CaseId { get; set; }
    public Case Case { get; set; } = null!;

    public int ContactId { get; set; }
    public Contact Contact { get; set; } = null!;

    public string Role { get; set; } = string.Empty;
    public string? Notes { get; set; }
}