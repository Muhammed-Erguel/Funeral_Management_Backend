namespace Funeral_Management_Backend.DTOs.Contact;

public class CreateContactDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? CompanyName { get; set; }

    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Language { get; set; }
    public string? Notes { get; set; }
}