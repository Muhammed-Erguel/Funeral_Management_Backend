namespace Funeral_Management_Backend.DTOs.CaseLocation;

public class AddCaseLocationDto
{
    public int LocationId { get; set; }

    public string Role { get; set; } = string.Empty;
    public string? Notes { get; set; }
}