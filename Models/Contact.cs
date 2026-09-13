namespace Funeral_Management_Backend.Models;

public class Contact
{
    public int Id { get; set; }

    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? CompanyName { get; set; }

    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Language { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CaseContact> CaseContacts { get; set; } = new List<CaseContact>();
}