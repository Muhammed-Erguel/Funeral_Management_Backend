namespace Funeral_Management_Backend.Models;

public class TaskItem
{
    public int Id { get; set; }

    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public int? CaseId { get; set; }
    public Case? Case { get; set; }

    public int? LocationId { get; set; }
    public Location? Location { get; set; }

    public int CreatedBy { get; set; }
    public User CreatedByUser { get; set; } = null!;

    public string? Kind { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public DateTime? DueAt { get; set; }
    public DateTime? ReminderAt { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}