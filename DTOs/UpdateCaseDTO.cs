namespace Funeral_Management_Backend.DTOs.Case;

public class UpdateCaseDto
{
    public string? Status { get; set; }

    public string? DeceasedFirstName { get; set; }
    public string? DeceasedLastName { get; set; }

    public DateOnly? DateOfBirth { get; set; }
    public DateOnly? DateOfDeath { get; set; }

    public string? DestinationCountry { get; set; }
    public string? DestinationCity { get; set; }
}