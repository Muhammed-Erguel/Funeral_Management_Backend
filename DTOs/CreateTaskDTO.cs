namespace Funeral_Management_Backend.DTOs.Task;

public class CreateTaskDto
{
    public int? CaseId { get; set; }
    public int? LocationId { get; set; }

    public string? Kind { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public DateTime? DueAt { get; set; }
    public DateTime? ReminderAt { get; set; }

    public string Status { get; set; } = string.Empty;
}