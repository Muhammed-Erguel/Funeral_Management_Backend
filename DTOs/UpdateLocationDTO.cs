namespace Funeral_Management_Backend.DTOs.Location;

public class UpdateLocationDto
{   
    public string? Name { get; set; }
    public string? Type { get; set; }

    public string? Street { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? OpeningHours { get; set; }
    public string? Notes { get; set; }
}