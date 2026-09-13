namespace Funeral_Management_Backend.Models;

public class CaseLocation
{
    public int CaseId { get; set; }
    public Case Case { get; set; } = null!;

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public string Role { get; set; } = string.Empty;
    public string? Notes { get; set; }
}