namespace Funeral_Management_Backend.DTOs.Case;

public class CaseResponseDto
{
    public int Id { get; set; }

    public string CaseNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public string DeceasedFirstName { get; set; } = string.Empty;
    public string DeceasedLastName { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }
    public DateOnly? DateOfDeath { get; set; }

    public string? DestinationCountry { get; set; }
    public string? DestinationCity { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}