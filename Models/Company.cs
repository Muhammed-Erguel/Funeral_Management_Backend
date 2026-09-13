namespace Funeral_Management_Backend.Models;

using System.Xml.Linq;

public class Company
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Street { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Case> Cases { get; set; } = new List<Case>();
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    public ICollection<Location> Locations { get; set; } = new List<Location>();
    public ICollection<DocumentType> DocumentTypes { get; set; } = new List<DocumentType>();
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}