using System.Reflection.Metadata;

namespace Funeral_Management_Backend.Models;

public class Case
{
    public int Id { get; set; }

    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public string CaseNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public string DeceasedFirstName { get; set; } = string.Empty;
    public string DeceasedLastName { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }
    public DateOnly? DateOfDeath { get; set; }

    public string? DestinationCountry { get; set; }
    public string? DestinationCity { get; set; }

    public int CreatedBy { get; set; }
    public User CreatedByUser { get; set; } = null!;

    public int? UpdatedBy { get; set; }
    public User? UpdatedByUser { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<CaseContact> CaseContacts { get; set; } = new List<CaseContact>();
    public ICollection<CaseLocation> CaseLocations { get; set; } = new List<CaseLocation>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}