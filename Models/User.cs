using System.Reflection.Metadata;

namespace Funeral_Management_Backend.Models;

public class User
{
    public int Id { get; set; }

    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Case> CreatedCases { get; set; } = new List<Case>();
    public ICollection<Case> UpdatedCases { get; set; } = new List<Case>();

    public ICollection<Document> UploadedDocuments { get; set; } = new List<Document>();
    public ICollection<TaskItem> CreatedTasks { get; set; } = new List<TaskItem>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}