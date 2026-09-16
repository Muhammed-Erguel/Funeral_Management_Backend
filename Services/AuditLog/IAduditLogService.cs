namespace Funeral_Management_Backend.Services.Audit;

public interface IAuditLogService
{
    Task LogAsync(
        string action,
        string entityType,
        int? entityId = null,
        int? caseId = null,
        object? metadata = null);
}