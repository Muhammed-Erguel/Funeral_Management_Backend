namespace Funeral_Management_Backend.Models;

public class AuditLog
{
    public int Id { get; set; }

    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int? CaseId { get; set; }
    public Case? Case { get; set; }

    public string Action { get; set; } = string.Empty;

    public string EntityType { get; set; } = string.Empty;
    public int? EntityId { get; set; }

    public string? Metadata { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}